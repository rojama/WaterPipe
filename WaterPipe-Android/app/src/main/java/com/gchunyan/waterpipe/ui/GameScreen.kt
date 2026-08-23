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
import com.gchunyan.waterpipe.util.WaterErrBitmap
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
                    .weight(PipeTypes.YULANG_W.toFloat(), fill = false), vm, settings, sound, onFinalizeRequested, onNewGame,
                    yulangBitmap)

                BoxWithConstraints(Modifier.fillMaxHeight().weight(PipeTypes.WANGGE_W.toFloat(), fill = false)) {
                    GridPanel(Modifier.fillMaxSize(), vm, settings, animState, ticker, wanggeBitmap) { r, c ->
                        if (!vm.engine.isFinalizing) {
                            val res = vm.engine.putImage(r, c)
                            if (res.ok && settings.soundsOn) {
                                val sfx = when (res.type) {
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

    Box(mod) {
        // yulang 背景：高度与内容一致（分数区+滚动格+跳过按钮），不纵向拉伸，顶部对齐
        yulang?.let {
            Image(bitmap = it, contentDescription = null,
                modifier = Modifier.matchParentSize(),
                contentScale = ContentScale.FillHeight,
                alignment = Alignment.TopCenter)
        }

        // 内容层：与 yulang 背景顶端对齐
        Column(
            Modifier
                .fillMaxSize()
                .padding(horizontal = 6.dp),
            verticalArrangement = Arrangement.spacedBy(4.dp)
        ) {
            // 分数区
            Text("分数: ${engine.score}", fontSize = 18.sp,
                fontWeight = FontWeight.Bold, color = Color(0xFF1976D2))

            Text("剩余: ${engine.remaining}", fontSize = 18.sp,
                fontWeight = FontWeight.Bold, color = Color(0xFF0D47A1))

            // 滚动预览5格：按固定正方形尺寸排列，不拉伸
            if (s.queueDirectionUp) {
                PreviewColumn(vm, s, Modifier)
            } else {
                PreviewColumnDown(vm, s, Modifier)
            }

            // 跳过按钮紧挨滚动格子下方
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
                contentPadding = androidx.compose.foundation.layout.PaddingValues(
                    horizontal = 4.dp, vertical = 2.dp),
                modifier = Modifier.fillMaxWidth()
            ) {
                Text(txt, fontSize = 12.sp, maxLines = 1)
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
    /** 当前正在播放水流动画的格子。每波动画结束即被移入 [doneSegments]。 */
    val segments: MutableMap<Int, MutableList<SegmentProgress>> = mutableMapOf()
    /** 已注水完成的格子 → 常显水纹动画段（frame 固定为最终帧，不再被后续波清除）。 */
    val doneSegments: MutableMap<Int, MutableList<SegmentProgress>> = mutableMapOf()
    var errSplashStep = 0
    var errBoxes: List<Pair<Int, Char>> = emptyList()
    var perfectBonusShown = false

    fun clear() {
        segments.clear(); doneSegments.clear(); errBoxes = emptyList(); errSplashStep = -1; perfectBonusShown = false
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
                val wavePlans = result.animatedBoxes
                val transient = HashMap<Int, MutableList<AnimationState.SegmentProgress>>()
                for (plan in wavePlans) {
                    val segs = plan.segments.map { (a, b) ->
                        AnimationState.SegmentProgress(a, b, 0, PipeTypes.ANIM_FRAMES)
                    }.toMutableList()
                    transient[plan.box] = segs
                    anim.segments[plan.box] = segs
                }

                for (f in 1..PipeTypes.ANIM_FRAMES) {
                    if (localReset != resetKey.value) throw CancellationException()
                    for (vs in anim.segments.values) vs.forEach { it.frame = f }
                    ticker.trySend(Unit)
                    delay(PipeTypes.ANIM_FRAME_MS)
                }

                // 本波动画播放完：把已注水的格子固定为最终帧并登记为常显，后续波不再清除
                for ((box, segs) in transient) {
                    segs.forEach { it.frame = PipeTypes.ANIM_FRAMES }
                    anim.doneSegments[box] = segs
                }
            }
            // 清除瞬态动画段，避免残留；常显水纹放在 doneSegments 中继续显示
            anim.segments.clear()

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
        for (u in ticker) frame++
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
                androidx.compose.ui.unit.DpSize(widthFromHeight, maxH)
            } else {
                androidx.compose.ui.unit.DpSize(maxW, heightFromWidth)
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
                                    frame = frame,
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
    frame: Long,
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

        // frame 读取触发每帧重组（动画渲染）
        val doing = anim.segments[box]
        val done = anim.doneSegments[box]
        val segs = if (frame >= 0) (doing ?: done) else null
        if (segs != null) {
            androidx.compose.foundation.Canvas(Modifier.fillMaxSize()) {
                drawWaterOverlay(segs, tag, this)
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
    tag: String,
    scope: DrawScope
) {
    val atlas = WaterAnimBitmap.get() ?: return
    val paint = androidx.compose.ui.graphics.Paint().apply {
        isAntiAlias = true
    }

    val cellW = scope.size.width
    val cellH = scope.size.height
    val sx = cellW / 75f
    val sy = cellH / 75f

    val frame = segs.firstOrNull()?.frame?.coerceIn(0, 15) ?: return
    if (frame <= 0) return

    val isStraightType = PipeTypes.isStraightAnimTag(tag)

    val srcRect = GRect()
    val dstRect = RectF()

    for (seg in segs) {
        val from = seg.from
        val to = seg.to

        if (from == to) {
            // 死胡同：入口向中心画直线（叠加 1..frame）
            drawLineAccum(atlas, paint, cellW, cellH, sx, sy, srcRect, dstRect,
                portToFlow(from, true), frame)
        } else if (isStraightType) {
            // 直线型管段（直线 / T 型 / 十字 / 桥梁）：
            //  阶段一 先入中心（帧 1..8，边缘→中心）
            //  阶段二 到中心后散向各出口（帧 9..15，中心→出口边缘）
            val entryCount = frame.coerceAtMost(PipeTypes.ANIM_HALF_FRAME)
            if (entryCount > 0) {
                drawLineAccum(atlas, paint, cellW, cellH, sx, sy, srcRect, dstRect,
                    portToFlow(from, true), entryCount)
            }
            if (frame > PipeTypes.ANIM_HALF_FRAME) {
                // 按剩余帧数（15-8=7 帧）铺满 8 片到达边缘
                val exitCount = (frame - PipeTypes.ANIM_HALF_FRAME) * 8 / (PipeTypes.ANIM_FRAMES - PipeTypes.ANIM_HALF_FRAME)
                if (exitCount > 0) {
                    drawLineAccumCenterOut(atlas, paint, cellW, cellH, sx, sy, srcRect, dstRect,
                        portToFlow(to, false), exitCount)
                }
            }
        } else {
            // 弧线型（L 型 / 双弧立交）：弧线累积
            val maxFrame = if (segs.size > 1) (frame * 2).coerceAtMost(15) else frame
            drawArcAccum(atlas, paint, cellW, cellH, sx, sy, srcRect, dstRect,
                from, to, maxFrame)
        }
    }
}

/** 端口字母 + 入/出 → SubLineAnimo 方向符 (E/W/S/N) */
private fun portToFlow(port: Char, isEntry: Boolean): Char = when (port) {
    'L' -> if (isEntry) 'E' else 'W'
    'R' -> if (isEntry) 'W' else 'E'
    'U' -> if (isEntry) 'S' else 'N'
    'D' -> if (isEntry) 'N' else 'S'
    else -> ' '
}

/** 叠加画 1..maxFrame 的 SubLineAnimo 切片 */
private fun DrawScope.drawLineAccum(
    atlas: android.graphics.Bitmap,
    paint: androidx.compose.ui.graphics.Paint,
    cellW: Float, cellH: Float, sx: Float, sy: Float,
    srcRect: GRect, dstRect: RectF,
    flowChar: Char, maxFrame: Int
) {
    for (f in 1..maxFrame) {
        srcRect.set(WaterAnimBitmap.lineSrcRect(flowChar, f))
        when (flowChar) {
            'E' -> {
                val x = (f - 1) * 5f * sx
                dstRect.set(x, 0f, x + 5f * sx, cellH)
            }
            'W' -> {
                val x = cellW - f * 5f * sx
                dstRect.set(x, 0f, x + 5f * sx, cellH)
            }
            'S' -> {
                val y = (f - 1) * 5f * sy
                dstRect.set(0f, y, cellW, y + 5f * sy)
            }
            'N' -> {
                val y = cellH - f * 5f * sy
                dstRect.set(0f, y, cellW, y + 5f * sy)
            }
            else -> continue
        }
        drawContext.canvas.nativeCanvas.drawBitmap(atlas, srcRect, dstRect, paint.asFrameworkPaint())
    }
}

/** 从中心向出口方向累积画 count 片直线水条（片 1..count 从中心向外延伸）。*/
private fun DrawScope.drawLineAccumCenterOut(
    atlas: android.graphics.Bitmap,
    paint: androidx.compose.ui.graphics.Paint,
    cellW: Float, cellH: Float, sx: Float, sy: Float,
    srcRect: GRect, dstRect: RectF,
    flowChar: Char, count: Int
) {
    val cx = cellW / 2f
    val cy = cellH / 2f
    for (f in 1..count) {
        srcRect.set(WaterAnimBitmap.lineSrcRect(flowChar, f))
        when (flowChar) {
            'E' -> {
                val x = cx + (f - 1) * 5f * sx
                dstRect.set(x, 0f, x + 5f * sx, cellH)
            }
            'W' -> {
                val x = cx - f * 5f * sx
                dstRect.set(x, 0f, x + 5f * sx, cellH)
            }
            'S' -> {
                val y = cy + (f - 1) * 5f * sy
                dstRect.set(0f, y, cellW, y + 5f * sy)
            }
            'N' -> {
                val y = cy - f * 5f * sy
                dstRect.set(0f, y, cellW, y + 5f * sy)
            }
            else -> continue
        }
        drawContext.canvas.nativeCanvas.drawBitmap(atlas, srcRect, dstRect, paint.asFrameworkPaint())
    }
}

/** 叠加画 1..maxFrame 的 SubArcAnimo 弧线切片。
 *  原 M8 弧线 dst 位置 (75px 坐标):
 *    LD/DL: (0, 20)   LU/UL: (0, 0)   UR/RU: (20, 0)   RD/DR: (20, 20)
 *  弧线大小: 55×55 (PIPE_MAIN_SIZE_ACR_IMG)
 */
private fun DrawScope.drawArcAccum(
    atlas: android.graphics.Bitmap,
    paint: androidx.compose.ui.graphics.Paint,
    cellW: Float, cellH: Float, sx: Float, sy: Float,
    srcRect: GRect, dstRect: RectF,
    from: Char, to: Char, maxFrame: Int
) {
    // 弧线 dst 位置 (按原 M8 代码, ARC_DESC=20, ARC_IMG=55)
    val pair = setOf(from, to)
    val dstX: Float
    val dstY: Float
    when {
        pair == setOf('L', 'D') -> { dstX = 0f;              dstY = 20f * sy }
        pair == setOf('L', 'U') -> { dstX = 0f;              dstY = 0f }
        pair == setOf('R', 'U') -> { dstX = 20f * sx;        dstY = 0f }
        pair == setOf('R', 'D') -> { dstX = 20f * sx;        dstY = 20f * sy }
        else -> { dstX = 0f; dstY = 0f }
    }
    val dstW = 55f * sx
    val dstH = 55f * sy

    for (f in 1..maxFrame) {
        srcRect.set(WaterAnimBitmap.arcSrcRect(from, to, f))
        dstRect.set(dstX, dstY, dstX + dstW, dstY + dstH)
        drawContext.canvas.nativeCanvas.drawBitmap(atlas, srcRect, dstRect, paint.asFrameworkPaint())
    }
}

private fun DrawScope.drawSplash(dirs: List<Char>, step: Int) {
    // 原版 ErrAnimo：51 步内重复 3 轮帧序列(0..16)，每步水滴喷出/降落
    val f = (step % WaterErrBitmap.FRAMES).coerceIn(0, WaterErrBitmap.FRAMES - 1)
    val left = WaterErrBitmap.frame(f, true) ?: return
    val right = WaterErrBitmap.frame(f, false) ?: return
    val paint = androidx.compose.ui.graphics.Paint().apply { isAntiAlias = true }
    val s = size.width / 75f
    val src = GRect(0, 0, WaterErrBitmap.FRAME_SIZE, WaterErrBitmap.FRAME_SIZE)
    val dst = RectF()
    val dpx = WaterErrBitmap.FRAME_SIZE * s

    for (d in dirs) {
        // 漏口侧壁中点（75 坐标）
        var descX = when (d) { 'U' -> 37.5f; 'R' -> 75f; 'D' -> 37.5f; else -> 0f }
        var descY = when (d) { 'L' -> 37.5f; 'R' -> 37.5f; 'D' -> 75f; else -> 0f }
        descX -= 45f; descY -= 65f
        var x1 = descX; var x2 = descX + 10f; var y = descY
        // 前6步水滴喷上并张开，之后下坠（同原版循环）
        for (m in 0..f) {
            if (m < 6) { x1 -= 4f; x2 += 4f; y -= 4f }
            else { x1 -= 3f; x2 += 3f; y += 10f }
        }
        dst.set(x1 * s, y * s, x1 * s + dpx, y * s + dpx)
        drawContext.canvas.nativeCanvas.drawBitmap(left, src, dst, paint.asFrameworkPaint())
        dst.set(x2 * s, y * s, x2 * s + dpx, y * s + dpx)
        drawContext.canvas.nativeCanvas.drawBitmap(right, src, dst, paint.asFrameworkPaint())
    }
}
