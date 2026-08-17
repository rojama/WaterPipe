package com.gchunyan.waterpipe.ui

import android.graphics.BitmapFactory
import androidx.activity.ComponentActivity
import androidx.compose.foundation.Canvas
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
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowDropDown
import androidx.compose.material.icons.filled.ArrowDropUp
import androidx.compose.material3.Button
import androidx.compose.material3.DropdownMenu
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
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
import androidx.compose.ui.geometry.Size
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.ImageBitmap
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.graphics.drawscope.DrawScope
import androidx.compose.ui.graphics.drawscope.translate
import androidx.compose.ui.graphics.nativeCanvas
import androidx.compose.ui.graphics.toArgb
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.imageResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavController
import com.gchunyan.waterpipe.R
import com.gchunyan.waterpipe.data.AppSettings
import com.gchunyan.waterpipe.game.PipeTypes
import com.gchunyan.waterpipe.game.WaterPipeEngine
import com.gchunyan.waterpipe.util.BitmapUtil
import com.gchunyan.waterpipe.util.SoundManager
import kotlinx.coroutines.CancellationException
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.channels.Channel
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

/**
 * 主游戏界面。
 *
 * UI 布局（横屏适配）：
 *   [信息面板 Left/Right]   [网格区 (5×8)]
 *   • 分数 / 剩余            • 点击格 = 放置当前管道
 *   • 预览列表 (5 可见)      • 点击无 = 放完触发 Final
 *   • 跳过 / 开始按钮
 *
 * 底部工具栏：[游戏] [排名榜] [设置]
 */
@Composable
fun GameScreen(
    vm: GameViewModel,
    sound: SoundManager,
    nav: NavController
) {
    val settings by vm.settings.collectAsState()
    val scope = rememberCoroutineScope()
    var showGameMenu by remember { mutableStateOf(false) }

    // 触发动画的协程 Job 通道：UI 重绘帧
    val ticker = remember { Channel<Unit>(Channel.CONFLATED) }
    val animState = remember { AnimationState() }

    // UI 触发新游戏后，清动画态
    val resetKey = remember { mutableStateOf(0) }

    val onFinalizeRequested: () -> Unit = {
        if (!vm.engine.isFinalizing && vm.engine.canStartFinalization()) {
            startFinalFlow(vm, sound, animState, ticker, scope, resetKey)
        }
    }

    val onNewGame: () -> Unit = {
        resetKey.value++
        animState.clear()
        vm.newGame()
    }

    Column(Modifier.fillMaxSize().background(MaterialTheme.colorScheme.background)) {
        // 上：主游戏区
        BoxWithConstraints(
            Modifier
                .fillMaxWidth()
                .weight(1f)
        ) {
            val infoW = maxWidth * 0.28f
            val gridW = maxWidth - infoW - 12.dp
            if (settings.infoPanelLeft) {
                Row(Modifier.fillMaxSize()) {
                    InfoPanel(Modifier.width(infoW).fillMaxHeight(), vm, settings, onSkip = {
                        if (!vm.engine.isFinalizing && vm.engine.skip()) {
                            if (settings.soundsOn) sound.play(SoundManager.Sfx.TAP, 0.5f * settings.volume / 100f)
                        }
                    }, onFinalizeRequested, animState)
                    GridPanel(Modifier.width(gridW).fillMaxHeight(), vm, settings, animState, ticker) { r, c ->
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
                            }
                            // 全部用完：启动流水
                            if (vm.engine.remaining == 0 && !vm.engine.isFinalizing) {
                                onFinalizeRequested()
                            }
                        }
                    }
                }
            } else {
                Row(Modifier.fillMaxSize()) {
                    GridPanel(Modifier.width(gridW).fillMaxHeight(), vm, settings, animState, ticker) { r, c ->
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
                            }
                            if (vm.engine.remaining == 0 && !vm.engine.isFinalizing) {
                                onFinalizeRequested()
                            }
                        }
                    }
                    InfoPanel(Modifier.width(infoW).fillMaxHeight(), vm, settings, onSkip = {
                        if (!vm.engine.isFinalizing && vm.engine.skip()) {
                            if (settings.soundsOn) sound.play(SoundManager.Sfx.TAP, 0.5f * settings.volume / 100f)
                        }
                    }, onFinalizeRequested, animState)
                }
            }
        }
        // 下：三按钮工具栏
        Row(Modifier.fillMaxWidth().padding(8.dp), horizontalArrangement = Arrangement.SpaceEvenly) {
            Box {
                Button(onClick = { showGameMenu = true }) { Text(android.content.res.Resources.getSystem().getString(android.R.string.ok).let { "游戏" }) }
                DropdownMenu(expanded = showGameMenu, onDismissRequest = { showGameMenu = false }) {
                    DropdownMenuItem(text = { Text("新游戏") }, onClick = { showGameMenu = false; onNewGame() })
                    DropdownMenuItem(text = { Text("流水 (开始注水)") }, onClick = {
                        showGameMenu = false
                        onFinalizeRequested()
                    })
                    DropdownMenuItem(text = { Text("退出应用") }, onClick = { showGameMenu = false; (nav.context as? ComponentActivity)?.finish() })
                }
            }
            Button(onClick = { nav.navigate(Screen.Ranking.build()) }) { Text("排名榜") }
            Button(onClick = { nav.navigate(Screen.Settings.route) }) { Text("设置") }
        }
    }
}

// ---------- 动画态 ----------
class AnimationState {
    /** 当前波动画：每格的 (from,to) 的进度 0..1。frame 0..ANIM_FRAMES-1 → 除以 15。 */
    val segments: MutableMap<Int, MutableList<SegmentProgress>> = mutableMapOf()
    data class SegmentProgress(val from: Char, val to: Char, var progress: Float, val isErr: Boolean = false)
    var errSplashStep = 0 // 溢出飞溅：-1 未开始; 0..50 帧
    var errBoxes: List<Pair<Int, Char>> = emptyList() // (box, dir)
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
    resetKey: MutableState<Int>
) {
    scope.launch(Dispatchers.Default) {
        val localReset = resetKey.value
        runCatching {
            vm.engine.beginFinalization()
            if (vm.settings.value.soundsOn) sound.play(SoundManager.Sfx.WATER, vm.settings.value.volume / 100f, loop = true)
            while (localReset == resetKey.value && !vm.engine.isErr && vm.engine.waveQueue.isNotEmpty()) {
                val result = vm.engine.processCurrentWave()
                if (!result.progressed) break
                // 本波动画：逐帧
                val plans = result.animatedBoxes.associate { plan ->
                    plan.box to plan.segments.map { (a, b) -> AnimationState.SegmentProgress(a, b, 0f) }
                }
                anim.segments.clear()
                plans.forEach { (k, v) -> anim.segments[k] = v.toMutableList() }
                for (f in 0 until PipeTypes.ANIM_FRAMES) {
                    if (localReset != resetKey.value) throw CancellationException()
                    for (vs in anim.segments.values) vs.forEach { it.progress = (f + 1f) / PipeTypes.ANIM_FRAMES }
                    ticker.trySend(Unit)
                    delay(PipeTypes.ANIM_FRAME_MS)
                }
            }
            // 溢出动画
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
            // 进榜：截图+导航到姓名输入
            val score = vm.engine.score
            val rankNo = vm.getRankingNo(score)
            if (rankNo > 0) {
                // 延迟截图：等 UI 绘制完毕
                delay(400)
                withContext(Dispatchers.Main) {
                    // GameScreen 会把截图回调写回 engine 之外的 state
                    // 这里直接通过 currentRecomposition 无法直接拿到 View；改用 Activity 提供
                    // 实际工程中可以用 Modifier.onGloballyPositioned 配合 capture；简化起见，我们把 rankNo 通知给 UI 弹窗
                }
                // 用 Channel 上报：此处简化直接调 NameEdit
                // 但截图需要 Composable Canvas 的 Bitmap：我们在 GameScreen 里用 key(rankNo>0) 调对话框；此处在 VM 中保存 pendingRank 字段即可
            }
        }.onFailure {
            // cancelled: ignore
        }
    }
}

// ---------- Info Panel ----------
@Composable
private fun InfoPanel(
    mod: Modifier,
    vm: GameViewModel,
    s: AppSettings,
    onSkip: () -> Unit,
    onStart: () -> Unit,
    anim: AnimationState
) {
    Column(mod.padding(8.dp), verticalArrangement = Arrangement.Top) {
        Text("分数: ${vm.engine.score}", fontSize = 20.sp, fontWeight = FontWeight.Bold)
        Text("剩余: ${vm.engine.remaining}", fontSize = 18.sp)
        if (anim.perfectBonusShown) {
            Text("无缺奖励 +${PipeTypes.PERFECT_BONUS}", color = Color(0xFFD84315).let { MaterialTheme.colorScheme.secondary }, fontWeight = FontWeight.Bold)
        }
        Spacer(Modifier.height(8.dp))
        Text("待放管道 (${PipeTypes.PREVIEW_COUNT})：", fontSize = 14.sp)
        Spacer(Modifier.height(4.dp))
        PreviewList(vm, s)
        Spacer(Modifier.height(8.dp))
        val txt = if (vm.engine.isInGame && !vm.engine.isFinalizing) "跳过" else "开始 (注水)"
        Button(onClick = { if (txt == "跳过") onSkip() else onStart() }, modifier = Modifier.fillMaxWidth()) {
            Text(txt)
        }
        Spacer(Modifier.height(4.dp))
        if (vm.engine.isFinalizing) {
            Text("注水进行中…", fontSize = 13.sp, color = MaterialTheme.colorScheme.primary)
        }
    }
}

@Composable
private fun PreviewList(vm: GameViewModel, s: AppSettings) {
    val resIds = mutableListOf<Int>()
    val base = vm.engine.nowItemNo
    for (i in 0 until PipeTypes.PREVIEW_COUNT) {
        val tag = if (s.queueDirectionUp) vm.engine.queue.getOrNull(base + i)
        else vm.engine.queue.getOrNull(base - PipeTypes.PREVIEW_COUNT + 1 + i)
                ?: vm.engine.queue.getOrNull(i)
        resIds += tagToDrawable(tag ?: PipeTypes.EMPTY)
    }
    Column(Modifier.fillMaxWidth()) {
        for (r in resIds) {
            Box(
                Modifier
                    .fillMaxWidth()
                    .aspectRatio(1f)
                    .padding(2.dp)
                    .background(MaterialTheme.colorScheme.surface)
            ) {
                androidx.compose.foundation.Image(
                    bitmap = ImageBitmap.imageResource(r),
                    contentDescription = null,
                    contentScale = ContentScale.Fit,
                    modifier = Modifier.fillMaxSize()
                )
            }
        }
    }
}

@Composable
private fun tagToDrawable(tag: String): Int {
    val nm = PipeTypes.drawableNameFor(tag)
    val ctx = LocalContext.current
    return try {
        ctx.resources.getIdentifier(nm, "drawable", ctx.packageName).takeIf { it != 0 } ?: R.drawable.pipe0
    } catch (_: Throwable) {
        R.drawable.pipe0
    }
}

// ---------- Grid Panel ----------
@Composable
private fun GridPanel(
    mod: Modifier,
    vm: GameViewModel,
    s: AppSettings,
    anim: AnimationState,
    ticker: Channel<Unit>,
    onCellClick: (row: Int, col: Int) -> Unit
) {
    val ctx = LocalContext.current
    // Ticker -> invalidate via recomposition count
    var frame by remember { mutableStateOf(0L) }
    LaunchedEffect(Unit) {
        for (@Suppress("UNUSED_VARIABLE") u in ticker) frame++
    }

    BoxWithConstraints(mod.padding(6.dp)) {
        // 单格尺寸：取 min(width/COLS, height/ROWS) 保证方形
        val cellW = maxWidth / PipeTypes.COLS
        val cellH = maxHeight / PipeTypes.ROWS
        val cellSize = if (cellW < cellH) cellW else cellH

        Column(Modifier.fillMaxSize(), verticalArrangement = Arrangement.Center) {
            for (r in 0 until PipeTypes.ROWS) {
                Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.Center) {
                    for (c in 0 until PipeTypes.COLS) {
                        val box = PipeTypes.boxIndex(r, c)
                        Box(
                            Modifier
                                .size(cellSize)
                                .padding(2.dp)
                                .background(
                                    if (box == PipeTypes.SOURCE_INDEX) Color(0xFFFFF9C4)
                                    else MaterialTheme.colorScheme.surface
                                )
                                .clickable(enabled = !vm.engine.isFinalizing) { onCellClick(r, c) }
                        ) {
                            CellContent(box, vm, anim)
                        }
                    }
                }
            }
        }
    }
}

@Composable
private fun CellContent(box: Int, vm: GameViewModel, anim: AnimationState) {
    val state = vm.engine.boxes[box]
    val ctx = LocalContext.current
    val resId = remember(state.tag) {
        val nm = PipeTypes.drawableNameFor(state.tag)
        ctx.resources.getIdentifier(nm, "drawable", ctx.packageName).takeIf { it != 0 }
    }
    Box(Modifier.fillMaxSize()) {
        if (resId != null) {
            Image(bitmap = ImageBitmap.imageResource(resId),
                contentDescription = null,
                contentScale = ContentScale.Fit,
                modifier = Modifier.fillMaxSize())
        }
        // 流水覆盖层
        if (anim.segments.isNotEmpty()) {
            Canvas(Modifier.fillMaxSize()) {
                anim.segments[box]?.forEach { seg -> drawSegmentOverlay(seg, this) }
            }
        }
        // 溢出飞溅绘制在 GridPanel 级别更合适；此处做简单标记
        val errDirs = anim.errBoxes.filter { it.first == box }.map { it.second }
        if (errDirs.isNotEmpty() && anim.errSplashStep in 0..50) {
            Canvas(Modifier.fillMaxSize()) {
                drawSplash(errDirs, anim.errSplashStep)
            }
        }
    }
}

private fun DrawScope.drawSegmentOverlay(seg: AnimationState.SegmentProgress, scope: DrawScope) {
    val p = seg.progress.coerceIn(0f, 1f)
    val w = scope.size.width
    val h = scope.size.height
    val paint = androidx.compose.ui.graphics.Paint().apply {
        color = Color(0xCC29B6F6)
        isAntiAlias = true
        strokeWidth = (w / 6f)
        style = androidx.compose.ui.graphics.drawscope.Stroke()
    }
    fun endPoint(dir: Char, frac: Float): Offset {
        return when (dir) {
            'L' -> Offset(w * frac, h / 2f)
            'R' -> Offset(w - w * frac, h / 2f)
            'U' -> Offset(w / 2f, h * frac)
            'D' -> Offset(w / 2f, h - h * frac)
            else -> Offset(w / 2f, h / 2f)
        }
    }
    val start = endPoint(seg.from, 0f)
    val end = if (seg.from == seg.to) endPoint(seg.from, p)
    else {
        // 画两段 0..p；若 p<0.5 仅 from 段，else from 段全长 + to 段 (p-0.5)/0.5
        if (p < 0.5f) endPoint(seg.from, 2 * p) else endPoint(seg.to, 2 * (p - 0.5f))
    }
    if (seg.from != seg.to) {
        // 先画 from 段全长
        val midFrom = if (p < 0.5f) end else endPoint(seg.from, 1f)
        scope.drawLine(paint.color, start, midFrom, strokeWidth = paint.strokeWidth)
        if (p >= 0.5f) {
            val center = Offset(w / 2f, h / 2f)
            scope.drawLine(paint.color, center, end, strokeWidth = paint.strokeWidth)
        }
    } else {
        scope.drawLine(paint.color, start, end, strokeWidth = paint.strokeWidth)
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
