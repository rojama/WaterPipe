package com.gchunyan.waterpipe.game

/**
 * 管道类型常量：14 种 tag 字符串 + 资源 ID 映射。
 *
 * 方向说明：L=左 R=右 U=上 D=下
 * tag 字母顺序固定为 L<R<U<D，便于组合比较。
 *
 * （★）移除了原 DRM 依赖，tag 映射直接从资源 ID 得到，
 *     不再使用 MD5 机器码/注册码。
 */
object PipeTypes {

    const val COLS = 5
    const val ROWS = 8
    const val CELL_COUNT = COLS * ROWS        // 40
    const val QUEUE_SIZE = 45                 // 随机方块总数
    const val PREVIEW_COUNT = 5               // 预览可见数
    const val SOURCE_INDEX = 2                // 水源格索引：第 1 行第 3 格 (0,2)
    const val SOURCE_ENTRY = "U"              // 水从上方进入水源格

    // 左右面板原始像素宽度（M8 480x720 对应 480x615 工作区）
    const val YULANG_W = 90                   // 左侧面板宽
    const val WANGGE_W = 390                  // 网格面板宽
    const val WORK_W = YULANG_W + WANGGE_W    // 480

    const val ANIM_FRAMES = 15
    const val ANIM_HALF_FRAME = 8
    const val ANIM_FRAME_MS = 100L
    const val MAX_ANIM_THREADS = 20
    const val PERFECT_BONUS = 20               // 全程无缺奖励
    const val RANKING_MAX = 20

    /** 14 种基础管道 tag。*/
    const val LR = "LR"
    const val UD = "UD"
    const val LUR = "LUR"
    const val URD = "URD"
    const val LRD = "LRD"
    const val LUD = "LUD"
    const val LURD = "LURD"
    const val LURDX = "LURDX"
    const val LU = "LU"
    const val RU = "RU"
    const val RD = "RD"
    const val LD = "LD"
    const val LURD_BACK = "LURD\\"
    const val LURD_SLASH = "LURD/"
    const val EMPTY = ""
    const val SOURCE = "SRC"        // 水源格专用 tag：入口 + 管道口图
    const val SOURCE_TAG = SOURCE

    /** 随机生成管道：从 12 种普通型（不含三种立交）抽取 index 0..11。*/
    val RANDOM_TAGS: List<String> = listOf(
        LR, UD, LUR, URD, LRD, LUD, LURD, LURDX, LU, RU, RD, LD
    )

    /** tag -> drawable 资源名（与 drawable-nodpi 下的文件名对应，不含扩展名）。
     * 空格 (EMPTY) 返回 null — 原 C++ 代码 SetImage_Normal(NULL)，空格显示 WangGe 底图。
     * SOURCE (水源格) 用 pipe0 (入口图) */
    fun drawableNameFor(tag: String): String? = when (tag) {
        SOURCE -> "pipe0"
        LR -> "pipe1"
        UD -> "pipe2"
        LUR -> "pipe3"
        URD -> "pipe4"
        LRD -> "pipe5"
        LUD -> "pipe6"
        LURD -> "pipe7"
        LURDX -> "pipe8"
        LU -> "pipe9"
        RU -> "pipe10"
        RD -> "pipe11"
        LD -> "pipe12"
        LURD_BACK -> "pipe13"
        LURD_SLASH -> "pipe14"
        else -> null
    }

    /** 立交桥合并：旧 tag + 新 tag 方向互补则升级。
     * 规则：
     *  1. 两个半弧（如 LU+RD 或 LD+RU）→ 合并为双弧立交 (LURD_BACK / LURD_SLASH)
     *  2. 已有完整管 (LURD/LURDX) 不与任何管合并
     *  3. 方向有重叠的半弧不合并 → 替换 + 玻璃破碎音效
     *  4. 两个双弧立交 (BACK+SLASH) 方向不重叠 → 升级为桥梁 LURDX */
    fun mergeIfCompatible(oldTag: String, newTag: String): String? {
        val oldDirs = oldTag.toSet()
        val newDirs = newTag.toSet()

        // 已有完整十字管或桥梁，不合并
        if (oldTag == LURD || oldTag == LURDX ||
            newTag == LURD || newTag == LURDX) return null

        // Case 1: 两个管道方向不重叠，合起来恰好 {L,U,R,D} → 可合并
        val unionSet = oldDirs union newDirs
        if (unionSet == setOf('L', 'U', 'R', 'D')) {
            val intersection = oldDirs intersect newDirs
            if (intersection.isNotEmpty()) {
                return null  // 方向重叠，不合并
            }

            // 区分：两直管(LR+UD) → 十字 LURD；两半弧 → 双弧立交
            val isStraight = oldDirs == setOf('L', 'R') || oldDirs == setOf('U', 'D')
            if (isStraight) {
                return LURD  // 直管交叉 → 十字
            }

            // 两半弧：LU+RD → SLASH(/)，LD+RU → BACK(\)
            // LURD_BACK(\) = arcs {L,D}+{R,U}; LURD_SLASH(/) = arcs {L,U}+{R,D}
            val isBack = (oldDirs.contains('L') && oldDirs.contains('D')) ||
                         (oldDirs.contains('R') && oldDirs.contains('U'))
            return if (isBack) LURD_BACK else LURD_SLASH
        }

        // Case 2: 两个双弧立交方向不重叠 → 升级为桥梁 LURDX
        // BACK arcs: {L,U} + {R,D}
        // SLASH arcs: {L,D} + {R,U}
        // BACK + SLASH: arcs don't overlap → bridge LURDX
        if ((oldTag == LURD_BACK && newTag == LURD_SLASH) ||
            (oldTag == LURD_SLASH && newTag == LURD_BACK)) {
            return LURDX
        }

        return null
    }

    /** 单格计分：严格按原 README 规则。*/
    fun scoreFor(tag: String, inLab: String): Int {
        return when (tag) {
            LR, UD -> 7
            LUR, URD, LRD, LUD -> 10
            LURD -> 13
            LURDX -> {
                val horizontal = inLab.contains('L') || inLab.contains('R')
                val vertical = inLab.contains('U') || inLab.contains('D')
                if (horizontal && vertical) 16 else 8
            }
            LU, RU, RD, LD -> 6
            LURD_BACK -> {
                val d1 = (inLab.contains('L') && inLab.contains('D'))
                val d2 = (inLab.contains('R') && inLab.contains('U'))
                if (d1 && d2) 12 else 6
            }
            LURD_SLASH -> {
                val d1 = (inLab.contains('R') && inLab.contains('D'))
                val d2 = (inLab.contains('L') && inLab.contains('U'))
                if (d1 && d2) 12 else 6
            }
            else -> 0
        }
    }

    // ----- 坐标辅助 -----
    fun boxIndex(row: Int, col: Int): Int = row * COLS + col
    fun rowOf(box: Int): Int = box / COLS
    fun colOf(box: Int): Int = box % COLS

    /** 返回指定方向邻居索引，越界返回 null。
     * （修复原 GetBoxInDirection 返回 -2 的易错约定，改为可空。）*/
    fun neighbor(box: Int, direction: Char): Int? {
        val r = rowOf(box)
        val c = colOf(box)
        return when (direction) {
            'W' -> if (c > 0) box - 1 else null
            'E' -> if (c < COLS - 1) box + 1 else null
            'N' -> if (r > 0) box - COLS else null
            'S' -> if (r < ROWS - 1) box + COLS else null
            else -> null
        }
    }

    /** 当前方块接收来自 direction 方向的水流，返回 (nextBox?, nextInLab, 溢出报错方向?)。
     *  flowDir：水流离开本方方向 W/E/N/S -> nextBox 的 in 字母为对侧：
     *    W->邻居是西面->邻居 in="R" ; E->in="L" ; N->in="D" ; S->in="U" */
    data class FlowOut(val nextBox: Int?, val inLetter: Char, val errLetter: Char)

    fun flowOutMap(flowDir: Char): FlowOut = when (flowDir) {
        'W' -> FlowOut(null, 'R', 'L')      // 我方出口在西侧：对侧邻居入口=R；若出界→我方L口错
        'E' -> FlowOut(null, 'L', 'R')
        'N' -> FlowOut(null, 'D', 'U')
        'S' -> FlowOut(null, 'U', 'D')
        else -> error("invalid direction $flowDir")
    }
}
