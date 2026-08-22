package com.gchunyan.waterpipe.ui

import android.graphics.BitmapFactory
import android.graphics.Rect as GRect
import android.graphics.RectF
import androidx.activity.ComponentActivity
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.material3.DropdownMenu
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.ImageBitmap
import androidx.compose.ui.graphics.RectangleShape
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.graphics.drawscope.DrawScope
import androidx.compose.ui.graphics.nativeCanvas
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.imageResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavController
import com.gchunyan.waterpipe.R
import com.gchunyan.waterpipe.data.AppSettings
import com.gchunyan.waterpipe.game.PipeTypes
import com.gchunyan.waterpipe.game.WaterPipeEngine
import com.gchunyan.waterpipe.util.SoundManager
import com.gchunyan.waterpipe.util.WaterAnimBitmap
import kotlinx.coroutines.CancellationException
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.channels.Channel
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

@Composable
fun GameScreen(
    vm: GameViewModel,
    sound: SoundManager,
    nav: NavController
) {
    val settings by vm.settings.collectAsState()
    val scope = rememberCoroutineScope()
    var showGameMenu by remember { mutableStateOf(false) }

    val ticker = remember { Channel<Unit>(Channel.CONFLATED) }
    val animState = remember { AnimationState() }
    val resetKey = remember { mutableStateOf(0) }

    val onFinalizeRequested: () -> Unit = {
        if (!vm.engine.isFinalizing && vm.engine.canStartFinalization()) {
            startFinalFlow(vm, sound, animState, ticker, scope, resetKey, nav)
        }
    }

    val onNewGame: () -> Unit = {
        resetKey.value++
        animState.clear()
        vm.newGame()
    }

    val ctx = LocalContext.current
    val bgBitmap = remember {
        runCatching { BitmapFactory.decodeResource(ctx.resources, R.drawable.main_bg)?.asImageBitmap() }
            .getOrNull()
    }
    val yulangBitmap = remember {
        runCatching { BitmapFactory.decodeResource(ctx.resources, R.drawable.yulang)?.asImageBitmap() }
            .getOrNull()
    }
    val wanggeBitmap = remember {
        runCatching { BitmapFactory.decodeResource(ctx.resources, R.drawable.wangge)?.asImageBitmap() }
            .getOrNull()
    }

    // 背景拉伸满屏，覆盖状态栏区域
    Box(Modifier.fillMaxSize()) {
        bgBitmap?.let {
            Image(bitmap = it, contentDescription = null,
                modifier = Modifier.fillMaxSize(),
                contentScale = ContentScale.Crop)
        }

        Column(Modifier.fillMaxSize()) {
            Row(Modifier.weight(1f, fill = true)) {
                GameLeftPanel(Modifier
                    .fillMaxHeight()
                    .weight(PipeTypes.YULANG_W.toFloat(), fill = false), vm, settings, sound, onFinalizeRequested, onNewGame,
                    yulangBitmap)

                BoxWithConstraints(Modifier.fillMaxHeight().weight(PipeTypes.WANGGE_W.toFloat(), fill = false)) {
                    GridPanel(Modifier.fillMaxSize(), vm, settings, animState, ticker, wanggeBitmap) { r, c ->
                        if (!vm.engine.isFinalizing) {
                            val res = vm.engine.putImage(r, c)
                            if (res.ok && settings.soundsOn) {
                                val sfx = when (res.type) {
                                    WaterPipeEngine.PlaceType.MERGE -> SoundManager.Sfx.MERGE
                                    WaterPipeEngine.PlaceType.REPLACE -> SoundManager.Sfx.BREAK
                                    else -> SoundManager.Sfx.PLACE
                                }
                                sound.play(sfx, 0.6f * settings.volume / 100f)
                            }
                            if (res.ok) vm.notifyChanged()
                            if (vm.engine.remaining == 0 && !vm.engine.isFinalizing) {
                                onFinalizeRequested()
                            }
                        }
                    }
                }
            }

            BottomToolbar(Modifier.fillMaxWidth(), vm, onNewGame, onFinalizeRequested, nav)
        }
    }
}

@Composable
private fun GameLeftPanel(
    mod: Modifier,
    vm: GameViewModel,
    s: AppSettings,
    sound: SoundManager,
    onStart: () -> Unit,
    onNewGame: () -> Unit,
    yulang: ImageBitmap?
) {
    val engine = vm.engine

    BoxWithConstraints(mod) {
        // yulang 背景：按 wangge 高度基准等比缩放，顶端对齐
        yulang?.let {
            Image(bitmap = it, contentDescription = null,
                modifier = Modifier.fillMaxSize(),
                contentScale = ContentScale.FillBounds)
        }

        // 内容层：按 yulang 原图坐标比例定位
        Column(
            Modifier
                .fillMaxHeight()
                .padding(top = 0.dp)
                .padding(horizontal = 10.dp)
        ) {
            // 分数区（yulang 顶部 0~60px）
            Text("分数: ${engine.score}", fontSize = 18.sp,
                fontWeight = FontWeight.Bold, color = Color(0xFF1976D2))

            Text("剩余: ${engine.remaining}", fontSize = 18.sp,
                fontWeight = FontWeight.Bold, color = Color(0xFF0D47A1))

            Spacer(Modifier.height(14.dp))

            // 滚动网格区域 + 跳过按钮 (整体与右侧 wangge 顶端对齐)
            Box(Modifier.fillMaxHeight().weight(1f, fill = false)) {
                Column(Modifier.fillMaxSize()) {
                    // 预览5格在顶端对齐显示 (不拉伸, 权重为滚动区)
                    Box(Modifier.weight(1f, fill = false).fillMaxWidth()) {
                        if (s.queueDirectionUp) {
                            PreviewColumn(vm, s, Modifier.fillMaxSize())
                        } else {
                            PreviewColumnDown(vm, s, Modifier.fillMaxSize())
                        }
                    }

                    // 跳过按钮在滚动格子下方(背景内底部)
                    val txt = if (engine.isInGame && !engine.isFinalizing) "跳过" else "开始注水"
                    Button(
                        onClick = {
                            if (engine.isInGame && !engine.isFinalizing) {
                                if (engine.skip()) {
                                    if (s.soundsOn) sound.play(SoundManager.Sfx.TAP, 0.5f * s.volume / 100f)
                                    vm.notifyChanged()
                                }
                            } else {
                                onStart()
                            }
                        },
                        shape = RectangleShape,
                        modifier = Modifier
                            .fillMaxWidth()
                            .height(40.dp)
                    ) {
                        Text(txt, fontSize = 14.sp)
                    }
                }
            }
        }
    }
}

@Composable
private fun PreviewColumn(vm: GameViewModel, s: AppSettings, mod: Modifier) {
    val engine = vm.engine
    @Suppress("UNUSED_VARIABLE") val version = vm.gameVersion  // 读取 Compose state 触发重组
    Column(mod, verticalArrangement = Arrangement.spacedBy(4.dp)) {
        repeat(PipeTypes.PREVIEW_COUNT) { i ->
            val base = engine.nowItemNo
            val tag = engine.queue.getOrNull(base + i)
            val resId = tagToDrawableRes(tag)
            Box(
                Modifier
                    .fillMaxWidth()
                    .aspectRatio(1f, false)
                    .background(Color(0x22FFFFFF), RoundedCornerShape(4.dp))
            ) {
                if (resId != null) {
                    Image(bitmap = ImageBitmap.imageResource(resId),
                        contentDescription = null,
                        contentScale = ContentScale.Fit,
                        modifier = Modifier.fillMaxSize().padding(4.dp))
                }
            }
        }
    }
}

@Composable
private fun PreviewColumnDown(vm: GameViewModel, s: AppSettings, mod: Modifier) {
    val engine = vm.engine
    @Suppress("UNUSED_VARIABLE") val version = vm.gameVersion  // 读取 Compose state 触发重组
    Column(mod, verticalArrangement = Arrangement.spacedBy(4.dp)) {
        // 向下滚动模式：下一个水管在底部，之前的水管在上方
        // 显示顺序 (从上到下): [base+4, base+3, base+2, base+1, base]
        repeat(PipeTypes.PREVIEW_COUNT) { i ->
            val base = engine.nowItemNo
            val idx = base + (PipeTypes.PREVIEW_COUNT - 1 - i)
            val tag = engine.queue.getOrNull(idx)
            val resId = tagToDrawableRes(tag)
            Box(
                Modifier
                    .fillMaxWidth()
                    .aspectRatio(1f, false)
                    .background(Color(0x22FFFFFF), RoundedCornerShape(4.dp))
            ) {
                if (resId != null) {
                    Image(bitmap = ImageBitmap.imageResource(resId),
                        contentDescription = null,
                        contentScale = ContentScale.Fit,
                        modifier = Modifier.fillMaxSize().padding(4.dp))
                }
            }
        }
    }
}

@Composable
private fun tagToDrawableRes(tag: String?): Int? {
    if (tag == null) return null
    val nm = PipeTypes.drawableNameFor(tag) ?: return null
    val ctx = LocalContext.current
    val id = ctx.resources.getIdentifier(nm, "drawable", ctx.packageName)
    return if (id != 0) id else null
}

private fun tagToDrawableRes(ctx: android.content.Context, tag: String?): Int? {
    if (tag == null) return null
    val nm = PipeTypes.drawableNameFor(tag) ?: return null
    val id = ctx.resources.getIdentifier(nm, "drawable", ctx.packageName)
    return if (id != 0) id else null
}

@Composable
private fun BottomToolbar(
    mod: Modifier,
    vm: GameViewModel,
    onNewGame: () -> Unit,
    onFinalize: () -> Unit,
    nav: NavController
) {
    Row(
        mod
            .background(Color(0xFF0D47A1))
            .padding(vertical = 6.dp, horizontal = 8.dp),
        horizontalArrangement = Arrangement.SpaceEvenly
    ) {
        var showGameMenu by remember { mutableStateOf(false) }

        Box {
            androidx.compose.material3.TextButton(onClick = { showGameMenu = true }) {
                Text("游戏", color = Color.White, fontSize = 14.sp)
            }
            DropdownMenu(expanded = showGameMenu, onDismissRequest = { showGameMenu = false }) {
                DropdownMenuItem(text = { Text("新游戏") }, onClick = { showGameMenu = false; onNewGame() })
                DropdownMenuItem(text = { Text("流水 (开始注水)") }, onClick = { showGameMenu = false; onFinalize() })
                DropdownMenuItem(text = { Text("退出应用") },
                    onClick = { showGameMenu = false; (nav.context as? ComponentActivity)?.finish() })
            }
        }
        androidx.compose.material3.TextButton(onClick = {
            vm.refreshRanking()
            nav.navigate(Screen.Ranking.build())
        }) { Text("排名榜", color = Color.White, fontSize = 14.sp) }
        androidx.compose.material3.TextButton(onClick = { nav.navigate(Screen.Settings.route) }) {
            Text("设置", color = Color.White, fontSize = 14.sp)
        }
    }
}

class AnimationState {
    data class SegmentProgress(val from: Char, val to: Char, var frame: Int, val totalFrames: Int)
    val segments: MutableMap<Int, MutableList<SegmentProgress>> = mutableMapOf()
    var errSplashStep = 0
    var errBoxes: List<Pair<Int, Char>> = emptyList()
    var perfectBonusShown = false

    fun clear() {
        segments.clear(); errBoxes = emptyList(); errSplashStep = -1; perfectBonusShown = false
    }
}

private fun startFinalFlow(
    vm: GameViewModel,
    sound: SoundManager,
    anim: AnimationState,
    ticker: Channel<Unit>,
    scope: kotlinx.coroutines.CoroutineScope,
    resetKey: MutableState<Int>,
    nav: NavController
) {
    scope.launch(Dispatchers.Default) {
        val localReset = resetKey.value
        runCatching {
            vm.engine.beginFinalization()
            // 触发 UI 重新组合，使 CellView 能读取 isFinalizing=true，显示水源入口图
            withContext(Dispatchers.Main) { vm.notifyChanged() }
            if (vm.settings.value.soundsOn) sound.play(SoundManager.Sfx.WATER, vm.settings.value.volume / 100f, loop = true)

            while (localReset == resetKey.value && !vm.engine.isErr && vm.engine.waveQueue.isNotEmpty()) {
                val result = vm.engine.processCurrentWave()
                if (!result.progressed) break

                anim.segments.clear()
                for (plan in result.animatedBoxes) {
                    anim.segments[plan.box] = plan.segments.map { (a, b) ->
                        AnimationState.SegmentProgress(a, b, 0, PipeTypes.ANIM_FRAMES)
                    }.toMutableList()
                }

                for (f in 1..PipeTypes.ANIM_FRAMES) {
                    if (localReset != resetKey.value) throw CancellationException()
                    for (vs in anim.segments.values) vs.forEach { it.frame = f }
                    ticker.trySend(Unit)
                    delay(PipeTypes.ANIM_FRAME_MS)
                }
            }

            if (vm.engine.isErr) {
                sound.play(SoundManager.Sfx.WARNING, vm.settings.value.volume / 100f, loop = false)
                anim.errBoxes = vm.engine.errors.map { it.box to it.dir }
                for (k in 0..50) {
                    if (localReset != resetKey.value) throw CancellationException()
                    anim.errSplashStep = k
                    ticker.trySend(Unit)
                    delay(40L)
                }
            }

            val bonus = vm.engine.finishFinalization()
            if (bonus > 0) {
                anim.perfectBonusShown = true
                ticker.trySend(Unit)
            }
            sound.stopAll()

            withContext(Dispatchers.Main) {
                vm.notifyChanged()

                if (!vm.engine.isErr) {
                    val rank = vm.getRankingNo(vm.engine.score)
                    if (rank > 0) {
                        nav.navigate(Screen.NameEdit.build(rank, vm.engine.score))
                    }
                }
            }
        }.onFailure { }
    }
}

@Composable
private fun GridPanel(
    mod: Modifier,
    vm: GameViewModel,
    s: AppSettings,
    anim: AnimationState,
    ticker: Channel<Unit>,
    wangge: ImageBitmap?,
    onCellClick: (row: Int, col: Int) -> Unit
) {
    var frame by remember { mutableStateOf(0L) }
    LaunchedEffect(Unit) {
        for (@Suppress("UNUSED_VARIABLE") u in ticker) frame++
    }

    // 原资源 wangge 尺寸 390(W) x 615(H)
    val ratioW = PipeTypes.WANGGE_W.toFloat()
    val ratioH = 615f

    Box(mod.fillMaxSize(), contentAlignment = Alignment.Center) {
        BoxWithConstraints(Modifier.fillMaxSize().align(Alignment.Center)) {
            val maxW = maxWidth
            val maxH = maxHeight
            val widthFromHeight = maxH * (ratioW / ratioH)
            val heightFromWidth = maxW * (ratioH / ratioW)
            val target: androidx.compose.ui.unit.DpSize = if (widthFromHeight <= maxW) {
                androidx.compose.ui.unit.DpSize(widthFromHeight, maxH)   // 以高度为准 (竖直方向占满, 横屏时两侧留空)
            } else {
                androidx.compose.ui.unit.DpSize(maxW, heightFromWidth)   // 否则以宽度为准 (高窄屏时上下留空)
            }
            Box(Modifier.size(target.width, target.height)) {
                wangge?.let {
                    Image(bitmap = it, contentDescription = null,
                        modifier = Modifier.fillMaxSize(),
                        contentScale = ContentScale.FillBounds)
                }
                Column(Modifier.fillMaxSize()) {
                    repeat(PipeTypes.ROWS) { r ->
                        Row(Modifier.weight(1f, fill = true)) {
                            repeat(PipeTypes.COLS) { c ->
                                val box = PipeTypes.boxIndex(r, c)
                                CellView(
                                    mod = Modifier.weight(1f, fill = true),
                                    box = box,
                                    vm = vm,
                                    anim = anim,
                                    onClick = {
                                        if (!vm.engine.isFinalizing) onCellClick(r, c)
                                    }
                                )
                            }
                        }
                    }
                }
            }
        }
    }
}

@Composable
private fun CellView(
    mod: Modifier,
    box: Int,
    vm: GameViewModel,
    anim: AnimationState,
    onClick: () -> Unit
) {
    val version = vm.gameVersion  // 读取 Compose state 触发重组
    val state = vm.engine.boxes[box]
    val isFinalizing = vm.engine.isFinalizing
    val ctx = LocalContext.current
    val tag = state.tag

    // 水源格在游戏中不显示管道图，注水时才显示
    val displayTag = if (tag == PipeTypes.SOURCE && !isFinalizing) PipeTypes.EMPTY else tag
    val resId = remember(displayTag) { tagToDrawableRes(ctx, displayTag) }

    Box(
        mod
            .aspectRatio(1f, false)
            .clickable { onClick() }
    ) {
        if (resId != null) {
            Image(
                bitmap = ImageBitmap.imageResource(resId),
                contentDescription = null,
                contentScale = ContentScale.Fit,
                modifier = Modifier.fillMaxSize()
            )
        }

        if (anim.segments.containsKey(box)) {
            androidx.compose.foundation.Canvas(Modifier.fillMaxSize()) {
                drawWaterOverlay(anim.segments[box] ?: emptyList(), this)
            }
        }

        val errDirs = anim.errBoxes.filter { it.first == box }.map { it.second }
        if (errDirs.isNotEmpty() && anim.errSplashStep in 0..50) {
            androidx.compose.foundation.Canvas(Modifier.fillMaxSize()) {
                drawSplash(errDirs, anim.errSplashStep)
            }
        }
    }
}

private fun DrawScope.drawWaterOverlay(
    segs: List<AnimationState.SegmentProgress>,
    scope: DrawScope
) {
    val atlas = WaterAnimBitmap.get() ?: return
    val paint = androidx.compose.ui.graphics.Paint().apply {
        isAntiAlias = true
    }

    val cellW = scope.size.width
    val cellH = scope.size.height

    for (seg in segs) {
        val frame = seg.frame.coerceIn(0, seg.totalFrames)
        if (frame <= 0) continue
        drawSegmentUsingAtlas(atlas, paint, cellW, cellH, seg.from, seg.to, frame, seg.totalFrames)
    }
}

/**
 * 按原 M8 AllWater.png 素材 (15帧 SubLineAnimo + SubArcAnimo) 绘制单段流水 from->to。
 *  - 直管(对侧): 两段 SubLineAnimo, 前8帧入口->中心, 后7帧中心->出口
 *  - 半管(from==to): 单段 SubLineAnimo 1..frame 铺入
 *  - 弯管(非对侧):  SubLineAnimo(端口条) + SubArcAnimo(弯弧) 联合 15 帧铺满
 */
private fun DrawScope.drawSegmentUsingAtlas(
    atlas: android.graphics.Bitmap,
    paint: androidx.compose.ui.graphics.Paint,
    cellW: Float,
    cellH: Float,
    from: Char,
    to: Char,
    frame: Int,
    @Suppress("UNUSED_PARAMETER") totalFrames: Int
) {
    val halfFrame = PipeTypes.ANIM_HALF_FRAME
    val srcRect = GRect()
    val dstRect = RectF()

    val isStraightOpposite = (from == 'L' && to == 'R') || (from == 'R' && to == 'L') ||
                             (from == 'U' && to == 'D') || (from == 'D' && to == 'U')
    val isHalfPipe = (from == to)

    // SubLineAnimo 条尺寸: src 5x75 (横流) 或 75x5 (竖流). dst 按 cell/75 缩放.
    val barW_h = 5f * cellW / 75f   // 横流: 条宽(水平条的水平厚度)
    val barH_h = cellH              // 横流: 条高
    val barW_v = cellW              // 竖流: 条宽
    val barH_v = 5f * cellH / 75f   // 竖流: 条高(垂直条的竖直厚度)

    /** 使用 lineSrcRect (来自 AllWater row 4) 画一条 SubLineAnimo 的 frame 前沿窄条。
     *  flowChar: 'E' / 'W' / 'S' / 'N' 与原 M8 SubLineAnimo 方向一致 */
    fun drawLineBar(flowChar: Char, f: Int) {
        if (f <= 0 || f > 15) return
        srcRect.set(WaterAnimBitmap.lineSrcRect(flowChar, f))
        // 计算条在 cell 内的位置：15 帧时整格铺满 (前沿 = 端口 -> 中心 / 中心 -> 端口)
        val x: Float
        val y: Float
        val w: Float
        val h: Float
        val t = (f - 1) / 14f   // 0..1 表示从起始到对侧
        when (flowChar) {
            'E' -> { // 从左边界向右画
                x = (cellW - barW_h) * t
                y = 0f
                w = barW_h
                h = barH_h
            }
            'W' -> { // 从右边界向左画
                x = cellW - barW_h - (cellW - barW_h) * t
                y = 0f
                w = barW_h
                h = barH_h
            }
            'S' -> { // 从上边界向下画
                x = 0f
                y = (cellH - barH_v) * t
                w = barW_v
                h = barH_v
            }
            'N' -> { // 从下边界向上画
                x = 0f
                y = cellH - barH_v - (cellH - barH_v) * t
                w = barW_v
                h = barH_v
            }
            else -> { x = 0f; y = 0f; w = 0f; h = 0f }
        }
        dstRect.set(x, y, x + w, y + h)
        drawContext.canvas.nativeCanvas.drawBitmap(atlas, srcRect, dstRect, paint.asFrameworkPaint())
    }

    /** 用 SubArcAnimo (AllWater 1..3 行) 画一条弯管 from<->to 的弧线。*/
    fun drawArcBar(portA: Char, portB: Char, f: Int) {
        if (f <= 0 || f > 15) return
        val s = WaterAnimBitmap.arcSrcRect(portA, portB, f)
        srcRect.set(s)
        // arcSrcRect 返回的源矩形是从弧的"端口侧外端"到"中心"的弧线切片。
        // 在 75x75 tile 中, arc 条的源位置 y∈[17..41] (中段)
        // 我们把它等比缩放并贴到对应象限的弯管区域:
        //  四个弯管象限: LU (左上), RU (右上), LD (左下), RD (右下)
        // 每个象限是 cell 的 25%~50% 中心区域 (弧线从外边到中心弯)
        // 对于每一帧 f, arcSrcRect 的 x 偏移在 tile 范围内, 所以采样出的是弧的一小段;
        // 我们把这段画到对应象限内的 (1-f/15) 表示越靠近中心.
        val t = (f - 1) / 14f   // 0 (端口附近) -> 1 (中心)
        val sx = cellW / 75f
        val sy = cellH / 75f
        val srcW = srcRect.width().toFloat()
        val srcH = srcRect.height().toFloat()
        if (srcW <= 0 || srcH <= 0) return

        // 每个弯管组合对应的中心象限: 将 75x75 的 tile 的弯管条映射到 cell 对应的半段
        // a1=17, a2=41: arc条中心宽度, 对应 tile 中心附近
        // 将 arc 条当成位于 cell 中心轴上的一条带, 具体平移根据组合
        val pair = setOf(portA, portB)
        val centerX = cellW / 2f
        val centerY = cellH / 2f
        // dst 弧条: 先按比例缩放为 ~ dstW x dstH (与 src 相同方向)
        val dstW = srcW * sx
        val dstH = srcH * sy
        // 弯管弧线: 原 M8 的 SubArcAnimo 在 tile 的 arc 条绘制时,
        // frame 1 = 最靠近端口端, frame 15 = 最靠近中心端.
        // 我们直接将 arc bar 放到对应象限的 "中心附近 + 向端口外 t=0"
        val cx: Float
        val cy: Float
        when {
            pair == setOf('L', 'D') -> {
                // 左下方的弯: 中心在 (cellW*0.25, cellH*0.75) 附近 -> 移动到中心左下象限
                // frame 1 在端口端: L端=x=0附近, D端=y=cellH附近
                // frame 15 在中心
                cx = centerX - (1f - t) * (cellW * 0.40f)  // 越远越靠左
                cy = centerY + (1f - t) * (cellH * 0.40f)  // 越远越靠下
            }
            pair == setOf('L', 'U') -> {
                cx = centerX - (1f - t) * (cellW * 0.40f)
                cy = centerY - (1f - t) * (cellH * 0.40f)
            }
            pair == setOf('R', 'U') -> {
                cx = centerX + (1f - t) * (cellW * 0.40f)
                cy = centerY - (1f - t) * (cellH * 0.40f)
            }
            pair == setOf('R', 'D') -> {
                cx = centerX + (1f - t) * (cellW * 0.40f)
                cy = centerY + (1f - t) * (cellH * 0.40f)
            }
            else -> {
                cx = centerX
                cy = centerY
            }
        }
        dstRect.set(cx - dstW / 2f, cy - dstH / 2f, cx + dstW / 2f, cy + dstH / 2f)
        drawContext.canvas.nativeCanvas.drawBitmap(atlas, srcRect, dstRect, paint.asFrameworkPaint())
    }

    /** 把 port(L/R/U/D) 和 "入/出" 方向转换成 SubLineAnimo 的 flowChar (E/W/S/N). */
    fun portToFlow(port: Char, isEntry: Boolean): Char = when (port) {
        'L' -> if (isEntry) 'E' else 'W'
        'R' -> if (isEntry) 'W' else 'E'
        'U' -> if (isEntry) 'S' else 'N'
        'D' -> if (isEntry) 'N' else 'S'
        else -> ' '
    }

    // ========= 主分支 =========
    if (isHalfPipe) {
        // 死胡同/半截：只画 from 入口向中心推进 (1..15 frame)
        drawLineBar(portToFlow(from, isEntry = true), frame)
        return
    }

    if (isStraightOpposite) {
        // 直管: 前 8 帧 from -> 中心；后 7 帧 中心 -> to (衔接)
        val entryFrame = (frame * 2 - 1).coerceAtMost(15)
        if (frame <= halfFrame) {
            drawLineBar(portToFlow(from, isEntry = true), entryFrame)
        } else {
            // 入口端整段铺满
            drawLineBar(portToFlow(from, isEntry = true), 15)
            // 出口端逐步铺开
            val exitFrame = ((frame - halfFrame) * 2 - 1).coerceAtMost(15)
            drawLineBar(portToFlow(to, isEntry = false), exitFrame)
        }
        return
    }

    // 弯管 (非对侧 / 存在拐点). from -> 中心 -> to 三部分
    val entryFrame = if (frame <= halfFrame) {
        (frame * 2 - 1).coerceAtMost(15)
    } else {
        15
    }
    // from 端口直线条 (端口 -> 中心)
    drawLineBar(portToFlow(from, isEntry = true), entryFrame)
    // 弯管弧 1..15 (跨半帧衔接继续推进). 弧也分两段
    val arcFrame = when {
        frame <= halfFrame -> (frame * 2 - 1).coerceAtMost(15)
        else -> 15
    }
    drawArcBar(from, to, arcFrame)

    if (frame > halfFrame) {
        // to 端口出口直线条 (中心 -> 端口外)
        val exitFrame = ((frame - halfFrame) * 2 - 1).coerceAtMost(15)
        drawLineBar(portToFlow(to, isEntry = false), exitFrame)
    }
}

private fun DrawScope.drawSplash(dirs: List<Char>, step: Int) {
    val w = size.width; val h = size.height
    val paint = androidx.compose.ui.graphics.Paint().apply {
        color = Color(0xFFFFEB3B); isAntiAlias = true
    }
    for (d in dirs) {
        val start = when (d) {
            'L' -> Offset(0f, h / 2)
            'R' -> Offset(w, h / 2)
            'U' -> Offset(w / 2, 0f)
            'D' -> Offset(w / 2, h)
            else -> Offset(w / 2, h / 2)
        }
        val t = step / 50f
        val radius = 4f + t * 40f
        drawCircle(Color(0xFF29B6F6), radius = radius, center = start + Offset(
            (if (d == 'L') -1 else if (d == 'R') 1 else 0) * t * 40f,
            (if (d == 'U') -1 else if (d == 'D') 1 else 0) * t * 40f
        ))
    }
}
