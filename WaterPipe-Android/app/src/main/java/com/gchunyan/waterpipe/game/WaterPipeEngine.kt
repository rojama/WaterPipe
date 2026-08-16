package com.gchunyan.waterpipe.game

import kotlin.random.Random

/**
 * 核心游戏引擎：纯 Kotlin 数据+状态机，无 Android 依赖，便于单元测试。
 *
 * 职责：
 *  - 持有 5×8=40 格 BoxState
 *  - 持有 45 块待放置队列 + 当前偏移 nowItemNo
 *  - newGame / putImage(row,col) / skip / canFinal / runFinalWaveStep / nextWave
 *
 * （修复原代码 Bug）：
 *  1. 取消了全局未初始化句柄（原 C++ hFinalThread 未初始化问题）：使用显式 isRunning 标志 + 协程 Job。
 *  2. 原 NewGame 中对 in_lab 重复写问题 → 此处 inLab 用 Set 去重。
 *  3. 原 FinalThread 中 ThreadData 泄漏 → 引擎不持有线程，返回下一步动作给 UI 调度。
 *  4. 原 GetMobleKey 中双重析构 / SN 截断等 Bug → Android 版完全去除 DRM 相关代码。
 */
class WaterPipeEngine(
    private val seedProvider: () -> Long = { System.currentTimeMillis() }
) {

    val boxes: Array<BoxState> = Array(PipeTypes.CELL_COUNT) { BoxState() }
    /** 45 块队列，元素 = tag 字符串，超出后为 null。 */
    val queue: MutableList<String> = ArrayList(PipeTypes.QUEUE_SIZE)
    var nowItemNo: Int = 0
        private set
    var score: Int = 0
        private set
    /** 最终注水流程：当前待推进格索引队列（BFS）；空表示结束/未开始。 */
    val waveQueue: MutableList<Int> = mutableListOf()
    var isErr: Boolean = false
        private set
    val errors: MutableList<ErrBox> = mutableListOf()
    var isFinalizing: Boolean = false
        private set
    var isInGame: Boolean = true
        private set
    /** 无缺通关奖励触发标记。 */
    var perfectBonusGranted: Boolean = false
        private set

    val remaining: Int get() = (PipeTypes.QUEUE_SIZE - nowItemNo).coerceAtLeast(0)

    // ---------- 生命周期 ----------

    fun newGame() {
        // 重置状态
        isFinalizing = false
        isInGame = true
        isErr = false
        errors.clear()
        score = 0
        perfectBonusGranted = false
        waveQueue.clear()
        boxes.forEach {
            it.tag = PipeTypes.EMPTY
            it.inLab.clear()
            it.fullLab.clear()
        }
        nowItemNo = 0

        // 随机 45 管道队列
        val random = Random(seedProvider())
        queue.clear()
        repeat(PipeTypes.QUEUE_SIZE) {
            queue.add(PipeTypes.RANDOM_TAGS[random.nextInt(PipeTypes.RANDOM_TAGS.size)])
        }

        // 水源入口：第 1 行第 3 格 (0,2)，从顶部注入
        boxes[PipeTypes.SOURCE_INDEX].addIn(PipeTypes.SOURCE_ENTRY[0])
        waveQueue.add(PipeTypes.SOURCE_INDEX)
    }

    // ---------- 放置/跳过 ----------

    fun currentQueueTagOrNull(): String? = queue.getOrNull(nowItemNo)

    /** 跳过当前方块（不放置）。返回是否成功。 */
    fun skip(): Boolean {
        if (isFinalizing) return false
        if (nowItemNo >= PipeTypes.QUEUE_SIZE) return false
        nowItemNo++
        return true
    }

    /**
     * 在 (row, col) 放置当前方块。
     * @return 放置结果：成功(true)/失败(false)；成功同时 nowItemNo 自增。
     */
    fun putImage(row: Int, col: Int): Boolean {
        if (isFinalizing) return false
        if (row !in 0 until PipeTypes.ROWS || col !in 0 until PipeTypes.COLS) return false
        val tag = currentQueueTagOrNull() ?: return false
        val box = PipeTypes.boxIndex(row, col)
        val state = boxes[box]

        // 若该格已有 tag：尝试立交桥合并
        if (state.tag.isNotEmpty() && state.tag != PipeTypes.EMPTY) {
            val merged = PipeTypes.mergeIfCompatible(state.tag, tag)
            if (merged != null) {
                state.tag = merged
                nowItemNo++
                return true
            }
            // 不兼容：不可放置
            return false
        }
        // 空格：直接写入
        state.tag = tag
        nowItemNo++
        return true
    }

    /** 是否可以启动流水：全部 45 块使用完或玩家主动启动。 */
    fun canStartFinal(): Boolean = !isFinalizing && isInGame

    // ---------- 注水流程 ----------

    /** 启动注水 BFS 流程（准备 first check）。*/
    fun beginFinalization(): Boolean {
        if (isFinalizing) return false
        isFinalizing = true
        // 首步：校验水源格入口方向在 tag 中
        checkFirst()
        return true
    }

    private fun checkFirst() {
        for (box in waveQueue.toList()) {
            val st = boxes[box]
            for (entry in st.inLab) {
                if (!st.tag.contains(entry)) {
                    isErr = true
                    errors.add(ErrBox(box, entry))
                }
            }
        }
    }

    /**
     * 完成当前 waveQueue 中全部格子的"计分 + 推导下一波"。
     * 返回本波得分 + 下一波格子列表 + 本波要做动画的 (box, from, to) 列表；
     * 内部自动推进 waveQueue；当 waveQueue 空或 isErr 时 isFinalizing 不变，调用者调用 finishFinalization。
     *
     * @return WaveResult：本波结果；动画由 UI 层使用协程/Animator 推进。
     */
    data class WaveResult(
        val addedScore: Int,
        val animatedBoxes: List<FlowStepPlan>,
        val progressed: Boolean
    )
    data class FlowStepPlan(
        val box: Int,
        /** 每个 (from, to) 表示一条水流要通过本格。*/
        val segments: List<Pair<Char, Char>>
    )

    fun processCurrentWave(): WaveResult {
        if (!isFinalizing || waveQueue.isEmpty() || isErr) {
            return WaveResult(0, emptyList(), false)
        }
        val currentWave = waveQueue.toList()
        waveQueue.clear()
        val nextWave = mutableListOf<Int>()
        var added = 0
        val animatedPlans = mutableListOf<FlowStepPlan>()

        for (box in currentWave) {
            val st = boxes[box]
            val score = PipeTypes.scoreFor(st.tag, st.inLabAsString())
            added += score
            this.score += score

            val segs = buildSegmentsForAnimation(st)
            animatedPlans.add(FlowStepPlan(box, segs))
            st.fullLab.addAll(st.inLab)

            // 推导下一波
            val fromTag = st.tag
            val inLetters = st.inLab.toList()
            if (fromTag.isEmpty()) continue
            val outs = collectOutsForTag(fromTag, inLetters)
            // outs: List<Pair<flowDir:W/E/N/S, fromLetter>>
            for ((flowDir, _) in outs) {
                val map = PipeTypes.flowOutMap(flowDir)
                val nextBox = PipeTypes.neighbor(box, flowDir)
                if (nextBox != null) {
                    if (boxes[nextBox].tag.isEmpty()) continue  // 空格，水不能进入
                    boxes[nextBox].addIn(map.inLetter)
                    if (!nextWave.contains(nextBox)) nextWave.add(nextBox)
                } else {
                    // 边界溢出
                    isErr = true
                    errors.add(ErrBox(box, map.errLetter))
                }
            }
            // 立交独立双通道：LURDX 与 双弧立交，处理后清 in_lab 防止交叉传染
            if (fromTag == PipeTypes.LURDX || fromTag == PipeTypes.LURD_BACK || fromTag == PipeTypes.LURD_SLASH) {
                st.inLab.clear()
            }
        }

        waveQueue.addAll(nextWave)
        return WaveResult(added, animatedPlans, true)
    }

    /** 收尾：若未出错则给 PERFECT_BONUS；返回最终分差。 */
    fun finishFinalization(): Int {
        if (!isFinalizing) return 0
        var extra = 0
        if (!isErr && !perfectBonusGranted) {
            score += PipeTypes.PERFECT_BONUS
            extra = PipeTypes.PERFECT_BONUS
            perfectBonusGranted = true
        }
        isInGame = false
        return extra
    }

    // ---------- 内部辅助 ----------

    /** 按照 tag × in_lab，枚举 (流向方向 W/E/N/S, fromLetter) 列表。*/
    private fun collectOutsForTag(tag: String, inLetters: List<Char>): List<Pair<Char, Char>> {
        val out = mutableListOf<Pair<Char, Char>>()
        fun addAll(map: Map<Char, Char>) {
            for ((from, flowDir) in map) {
                if (from in inLetters) out.add(flowDir to from)
            }
        }
        when (tag) {
            PipeTypes.LR -> {
                // L 入 → 向东流；R 入 → 向西流
                addAll(mapOf('L' to 'E', 'R' to 'W'))
            }
            PipeTypes.UD -> {
                addAll(mapOf('U' to 'S', 'D' to 'N'))
            }
            PipeTypes.LUR -> {
                addAll(mapOf('L' to 'E', 'L' to 'N', 'U' to 'W', 'U' to 'E', 'R' to 'W', 'R' to 'N'))
                // 注意 T 型：任意两入→出第三口；单入→出另外两口
                // 若 in 中同时存在 L+U+R 则 无出口
                filterTeeOuts(inLetters, tag, out)
            }
            PipeTypes.URD -> {
                addAll(mapOf('U' to 'S', 'U' to 'E', 'R' to 'W', 'R' to 'S', 'D' to 'N', 'D' to 'E'))
                filterTeeOuts(inLetters, tag, out)
            }
            PipeTypes.LRD -> {
                addAll(mapOf('L' to 'E', 'L' to 'S', 'R' to 'W', 'R' to 'S', 'D' to 'W', 'D' to 'E'))
                filterTeeOuts(inLetters, tag, out)
            }
            PipeTypes.LUD -> {
                addAll(mapOf('L' to 'N', 'L' to 'S', 'U' to 'S', 'U' to 'W', 'D' to 'N', 'D' to 'W'))
                filterTeeOuts(inLetters, tag, out)
            }
            PipeTypes.LURD -> {
                addAll(mapOf('L' to 'E', 'L' to 'N', 'L' to 'S',
                             'U' to 'W', 'U' to 'E', 'U' to 'S',
                             'R' to 'W', 'R' to 'N', 'R' to 'S',
                             'D' to 'W', 'D' to 'N', 'D' to 'E'))
                filterCrossOuts(inLetters, tag, out)
            }
            PipeTypes.LURDX -> {
                // 过桥独立：横纵无关
                if ('L' in inLetters) out.add('E' to 'L')
                if ('R' in inLetters) out.add('W' to 'R')
                if ('U' in inLetters) out.add('S' to 'U')
                if ('D' in inLetters) out.add('N' to 'D')
            }
            PipeTypes.LU -> addAll(mapOf('L' to 'N', 'U' to 'W'))
            PipeTypes.RU -> addAll(mapOf('R' to 'N', 'U' to 'E'))
            PipeTypes.RD -> addAll(mapOf('R' to 'S', 'D' to 'E'))
            PipeTypes.LD -> addAll(mapOf('L' to 'S', 'D' to 'W'))
            PipeTypes.LURD_BACK -> {
                // LU + RD 双对角独立
                if ('L' in inLetters) out.add('N' to 'L')
                if ('U' in inLetters) out.add('W' to 'U')
                if ('R' in inLetters) out.add('S' to 'R')
                if ('D' in inLetters) out.add('E' to 'D')
            }
            PipeTypes.LURD_SLASH -> {
                // LD + RU 双对角独立
                if ('L' in inLetters) out.add('S' to 'L')
                if ('D' in inLetters) out.add('W' to 'D')
                if ('R' in inLetters) out.add('N' to 'R')
                if ('U' in inLetters) out.add('E' to 'U')
            }
        }
        return out
    }

    /** 丁字管：全入则无出口；两入则剩那一口；单入则剩两口。*/
    private fun filterTeeOuts(
        inLetters: List<Char>,
        tag: String,
        out: MutableList<Pair<Char, Char>>
    ) {
        val ports = tag.toList()
        val matched = ports.filter { it in inLetters }
        val outPorts = ports.filter { it !in inLetters }
        val keepFlows = mutableListOf<Pair<Char, Char>>()
        for (outPort in outPorts) {
            // 每个出口，仅在还有"对应入口"时留一条即可
            val rules = when (tag) {
                PipeTypes.LUR -> mapOf('L' to listOf('E','N'), 'U' to listOf('W','E'), 'R' to listOf('W','N'),
                                       '_' to emptyList())
                PipeTypes.URD -> mapOf('U' to listOf('S','E'), 'R' to listOf('W','S'), 'D' to listOf('N','E'),
                                       '_' to emptyList())
                PipeTypes.LRD -> mapOf('L' to listOf('E','S'), 'R' to listOf('W','S'), 'D' to listOf('W','E'),
                                       '_' to emptyList())
                PipeTypes.LUD -> mapOf('L' to listOf('N','S'), 'U' to listOf('S','W'), 'D' to listOf('N','W'),
                                       '_' to emptyList())
                else -> emptyMap()
            }
            for (from in matched) {
                val dirs = rules[from] ?: continue
                // 找到"该入口能出到该 outPort 的方向"
                val flows = when (tag) {
                    PipeTypes.LUR -> mapOf('L' to mapOf('R' to 'E', 'U' to 'N'),
                                           'U' to mapOf('L' to 'W', 'R' to 'E'),
                                           'R' to mapOf('L' to 'W', 'U' to 'N'))
                    PipeTypes.URD -> mapOf('U' to mapOf('R' to 'E', 'D' to 'S'),
                                           'R' to mapOf('U' to 'E', 'D' to 'S'),
                                           'D' to mapOf('U' to 'S', 'R' to 'E'))
                    PipeTypes.LRD -> mapOf('L' to mapOf('R' to 'E', 'D' to 'S'),
                                           'R' to mapOf('L' to 'W', 'D' to 'S'),
                                           'D' to mapOf('L' to 'W', 'R' to 'E'))
                    PipeTypes.LUD -> mapOf('L' to mapOf('U' to 'N', 'D' to 'S'),
                                           'U' to mapOf('L' to 'W', 'D' to 'S'),
                                           'D' to mapOf('L' to 'W', 'U' to 'N'))
                    else -> emptyMap()
                }
                val d = flows[from]?.get(outPort) ?: continue
                keepFlows.add(d to from)
            }
        }
        out.clear()
        out.addAll(keepFlows)
    }

    /** 十字 LURD：已进入的口不出；未进入的口对每个入口都能通向。入口全满则无出。 */
    private fun filterCrossOuts(
        inLetters: List<Char>,
        @Suppress("UNUSED_PARAMETER") tag: String,
        out: MutableList<Pair<Char, Char>>
    ) {
        val ports = listOf('L', 'R', 'U', 'D')
        val outPorts = ports.filter { it !in inLetters }
        val keep = mutableListOf<Pair<Char, Char>>()
        val flowMap = mapOf(
            'L' to mapOf('R' to 'E', 'U' to 'N', 'D' to 'S'),
            'R' to mapOf('L' to 'W', 'U' to 'N', 'D' to 'S'),
            'U' to mapOf('L' to 'W', 'R' to 'E', 'D' to 'S'),
            'D' to mapOf('L' to 'W', 'R' to 'E', 'U' to 'N')
        )
        for (from in inLetters) {
            for (to in outPorts) {
                val d = flowMap[from]?.get(to) ?: continue
                keep.add(d to from)
            }
        }
        out.clear()
        out.addAll(keep)
    }

    /** 生成 (from, to) 动画段列表。这里的 to 是端口字母，和 collectOuts 的方向字符不同。 */
    private fun buildSegmentsForAnimation(st: BoxState): List<Pair<Char, Char>> {
        val result = mutableListOf<Pair<Char, Char>>()
        val outs = collectOutsForTag(st.tag, st.inLab.toList())
        // 以 (from, outPort) 表达一段
        val flowMap = mapOf(
            'L' to mapOf('W' to 'L', 'E' to 'R', 'N' to 'U', 'S' to 'D'),
            'R' to mapOf('W' to 'L', 'E' to 'R', 'N' to 'U', 'S' to 'D'),
            'U' to mapOf('W' to 'L', 'E' to 'R', 'N' to 'U', 'S' to 'D'),
            'D' to mapOf('W' to 'L', 'E' to 'R', 'N' to 'U', 'S' to 'D')
        )
        for ((flowDir, from) in outs) {
            val outPort = when (flowDir) {
                'W' -> 'L'
                'E' -> 'R'
                'N' -> 'U'
                'S' -> 'D'
                else -> continue
            }
            result.add(from to outPort)
        }
        if (result.isEmpty() && st.inLab.isNotEmpty() && st.tag.isNotEmpty()) {
            // 没有下一波：所有入口都是"死胡同"，仍要绘制其入口段
            for (from in st.inLab) {
                result.add(from to from)
            }
        }
        return result
    }
}
