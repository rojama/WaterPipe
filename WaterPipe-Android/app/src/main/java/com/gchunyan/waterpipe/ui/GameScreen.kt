package com.gchunyan.waterpipe.ui

import android.app.Activity
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
import androidx.compose.foundation.layout.WindowInsets
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.layout.windowInsetsPadding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowDropDown
import androidx.compose.material.icons.filled.ArrowDropUp
import androidx.compose.material3.Button
import androidx.compose.material3.DropdownMenu
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.MutableState
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
import kotlin.math.roundToInt

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

    Box(
        Modifier
            .fillMaxSize()
            .windowInsetsPadding(WindowInsets.statusBars)
    ) {
        bgBitmap?.let {
            Image(bitmap = it, contentDescription = null,
                modifier = Modifier.fillMaxSize(),
                contentScale = ContentScale.Crop)
        }

        Column(Modifier.fillMaxSize()) {
            Row(Modifier.weight(1f, fill = true)) {
                GameLeftPanel(Modifier
                    .fillMaxHeight()
                    .widthIn(.82f), vm, settings, sound, onFinalizeRequested, onNewGame,
                    yulangBitmap)

                BoxWithConstraints(Modifier.fillMaxHeight().weight(1f)) {
                    GridPanel(Modifier.fillMaxSize(), vm, settings, animState, ticker, wanggeBitmap) { r, c ->
                        if (!vm.engine.isFinalizing) {
                            val ok = vm.engine.putImage(r, c)
                            if (ok) {
                                if (settings.soundsOn) {
                                    val tag = vm.engine.boxes[PipeTypes.boxIndex(r, c)].tag
                                    val merged = tag == PipeTypes.LURD_BACK || tag == PipeTypes.LURD_SLASH
                                    sound.play(
                                        if (merged) SoundManager.Sfx.MERGE else SoundManager.Sfx.PLACE,
                                        0.6f * settings.volume / 100f
                                    )
                                }
                                vm.notifyChanged()
                            }
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

private fun Modifier.widthIn(fraction: Float) =
    this.then(Modifier.run {
        val activity = androidx.compose.ui.platform.LocalContext.current as? Activity
        val sw = activity?.resources?.displayMetrics?.widthPixels ?: 1080
        width((sw * fraction).toInt().coerceAtLeast(180).dp)
    })

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
        yulang?.let {
            Image(bitmap = it, contentDescription = null,
                modifier = Modifier.fillMaxSize(),
                contentScale = ContentScale.FillBounds)
        }

        Column(
            Modifier
                .fillMaxSize()
                .padding(10.dp),
            verticalArrangement = Arrangement.spacedBy(6.dp)
        ) {
            val version = vm.gameVersion

            Text("分数: ${engine.score}", fontSize = 18.sp,
                fontWeight = FontWeight.Bold, color = Color(0xFF1976D2))

            Text("剩余: ${engine.remaining}", fontSize = 16.sp, color = Color(0xFF0D47A1))

            if (s.queueDirectionUp) {
                PreviewColumn(vm, s, Modifier.weight(1f, fill = false))
            } else {
                PreviewColumnDown(vm, s, Modifier.weight(1f, fill = false))
            }

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
                modifier = Modifier.fillMaxWidth().height(52.dp)
            ) {
                Text(txt, fontSize = 14.sp)
            }
        }
    }
}

@Composable
private fun PreviewColumn(vm: GameViewModel, s: AppSettings, mod: Modifier) {
    val version = vm.gameVersion
    val engine = vm.engine
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
    val version = vm.gameVersion
    val engine = vm.engine
    Column(mod, verticalArrangement = Arrangement.spacedBy(4.dp)) {
        repeat(PipeTypes.PREVIEW_COUNT) { i ->
            val base = engine.nowItemNo
            val idx = base - PipeTypes.PREVIEW_COUNT + 1 + i
            val tag = engine.queue.getOrNull(idx) ?: engine.queue.getOrNull(i)
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

private fun tagToDrawableRes(tag: String?): Int? {
    if (tag == null) return null
    val nm = PipeTypes.drawableNameFor(tag) ?: return null
    return try {
        android.content.Context().resources.getIdentifier(nm, "drawable", android.content.Context().packageName)
    } catch (_: Throwable) { null }
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

    val ctx = LocalContext.current
    val version = vm.gameVersion

    Box(mod.padding(4.dp)) {
        Column(Modifier.fillMaxSize(),
            verticalArrangement = Arrangement.SpaceEvenly,
            horizontalAlignment = Alignment.CenterHorizontally) {
            repeat(PipeTypes.ROWS) { r ->
                Row(Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceEvenly) {
                    repeat(PipeTypes.COLS) { c ->
                        val box = PipeTypes.boxIndex(r, c)
                        CellView(
                            mod = Modifier.weight(1f, fill = true),
                            box = box,
                            vm = vm,
                            anim = anim,
                            wangge = wangge,
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

@Composable
private fun CellView(
    mod: Modifier,
    box: Int,
    vm: GameViewModel,
    anim: AnimationState,
    wangge: ImageBitmap?,
    onClick: () -> Unit
) {
    val state = vm.engine.boxes[box]
    val ctx = LocalContext.current
    val tag = state.tag
    val resId = remember(tag) { tagToDrawableRes(ctx, tag) }

    Box(
        mod
            .aspectRatio(1f, false)
            .padding(1.dp)
            .clickable { onClick() }
    ) {
        if (wangge != null && box != PipeTypes.SOURCE_INDEX) {
            Image(bitmap = wangge, contentDescription = null,
                modifier = Modifier.fillMaxSize(),
                contentScale = ContentScale.Crop,
                alpha = 0.35f)
        }

        if (box == PipeTypes.SOURCE_INDEX) {
            Box(Modifier.fillMaxSize().background(Color(0xFFFFF9C4)))
        }

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
    val scale = cellW / 75f

    for (seg in segs) {
        val p = seg.frame.coerceIn(0, seg.totalFrames)
        if (p <= 0) continue
        drawLineSegmentOverlay(atlas, paint, cellW, cellH, seg.from, seg.to, p, seg.totalFrames, scale)
    }
}

private fun DrawScope.drawLineSegmentOverlay(
    atlas: android.graphics.Bitmap,
    paint: androidx.compose.ui.graphics.Paint,
    cellW: Float,
    cellH: Float,
    from: Char,
    to: Char,
    frame: Int,
    totalFrames: Int,
    scale: Float
) {
    // 端口到中心的方向映射
    val portDir: (Char) -> android.graphics.PointF = { port ->
        when (port) {
            'L' -> android.graphics.PointF(0f, cellH / 2f)
            'R' -> android.graphics.PointF(cellW, cellH / 2f)
            'U' -> android.graphics.PointF(cellW / 2f, 0f)
            'D' -> android.graphics.PointF(cellW / 2f, cellH)
            else -> android.graphics.PointF(cellW / 2f, cellH / 2f)
        }
    }
    val center = android.graphics.PointF(cellW / 2f, cellH / 2f)

    // 流水方向 (atlas SubLineAnimo 用 char 'E','W','N','S')
    fun flowDir(port: Char): Char = when (port) {
        'R' -> 'W'
        'L' -> 'E'
        'D' -> 'N'
        'U' -> 'S'
        else -> ' '
    }

    fun drawSegment(port: Char) {
        val dir = flowDir(port)
        val frameLenPx = (5 * scale).coerceAtLeast(1f)
        val tileSrcW = 5
        val tileSrcH = 75
        val row4Y = 75 * 4

        val srcRect = GRect()
        val dstRect = RectF()

        when (dir) {
            'E' -> {
                val totalSpan = cellW
                val drawnWidth = (frame.toFloat() / totalFrames) * totalSpan
                val startX = 0f
                srcRect.set(0, row4Y, tileSrcW, row4Y + tileSrcH)
                dstRect.set(startX, 0f, startX + frameLenPx, cellH)
            }
            'W' -> {
                val totalSpan = cellW
                val drawnWidth = (frame.toFloat() / totalFrames) * totalSpan
                val startX = totalSpan - drawnWidth
                srcRect.set(75 - tileSrcW, row4Y, 75, row4Y + tileSrcH)
                dstRect.set(startX, 0f, startX + frameLenPx, cellH)
            }
            'S' -> {
                val totalSpan = cellH
                val drawnHeight = (frame.toFloat() / totalFrames) * totalSpan
                val startY = 0f
                srcRect.set(0, row4Y, tileSrcH, row4Y + tileSrcW)
                dstRect.set(0f, startY, cellW, startY + frameLenPx)
            }
            'N' -> {
                val totalSpan = cellH
                val drawnHeight = (frame.toFloat() / totalFrames) * totalSpan
                val startY = totalSpan - drawnHeight
                srcRect.set(0, row4Y + tileSrcH - tileSrcW, 75, row4Y + tileSrcH)
                dstRect.set(0f, startY, cellW, startY + frameLenPx)
            }
        }
        drawContext.canvas.nativeCanvas.drawBitmap(atlas, srcRect, dstRect, paint.asFrameworkPaint())
    }

    if (from == to) {
        drawSegment(from)
        return
    }

    if (frame <= PipeTypes.ANIM_HALF_FRAME) {
        drawSegment(from)
    } else {
        drawSegment(from)
        drawSegment(to)
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
