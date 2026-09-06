package com.gchunyan.waterpipe.util

import android.content.Context
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import com.gchunyan.waterpipe.R

/**
 * AllWater.png 流水动画图集辅助。
 *
 * 原图 1125×375 = (15 × 75) × (5 × 75)。
 * Row 4 (y=300..375) 存直线水纹理。
 * Row 0..3 存 4 种弯管弧形水纹理。
 *
 * 原 M8 常量:
 *   PIPE_MAIN_SIZE_IMG = 75 (格子尺寸)
 *   PIPE_MAIN_SIZE_ACR_DESC = 20 (弧形偏移)
 *   PIPE_MAIN_SIZE_ACR_IMG = 55 (弧形大小)
 *   frame_length = 5 (直线帧步进)
 *   PIPE_ANIMO_FRAME = 15 (总帧数)
 *   PIPE_ANIMO_HAFE_FRAME = 8 (半帧)
 */
object WaterAnimBitmap {

    const val TILE = 75
    const val FRAME_LENGTH = 5
    const val ARC_DESC = 20    // PIPE_MAIN_SIZE_ACR_DESC
    const val ARC_IMG = 55    // PIPE_MAIN_SIZE_ACR_IMG
    const val ANIM_FRAMES = 15
    const val ANIM_HALF_FRAMES = 8

    private var atlas: Bitmap? = null

    fun ensureLoaded(ctx: Context) {
        if (atlas != null) return
        runCatching {
            val options = BitmapFactory.Options().apply { inPreferredConfig = Bitmap.Config.ARGB_8888 }
            atlas = BitmapFactory.decodeResource(ctx.resources, R.drawable.allwater, options)
        }
    }

    fun isLoaded(): Boolean = atlas != null
    fun get(): Bitmap? = atlas
    fun recycle() { atlas?.recycle(); atlas = null }

    /**
     * SubLineAnimo 源矩形 — 完全对应 M8 SubLineAnimo 函数。
     *
     * E: src=(5*(frame-1), 300), size=5×75  水平条
     * W: src=(75-5*frame, 300), size=5×75   水平条
     * S: src=(75, 300+5*(frame-1)), size=75×5  垂直条 (竖直水线网点第 2 tile)
     * N: src=(75, 375-5*frame), size=75×5     垂直条 (与 S 同网点，仅帧序反向)
     *
     * 注：第 4 行纹理只有两种网点——col0(x0..75)为水平水线(供 E/W)，col75(x75..150)为垂直水线(供 S/N)。
     *     S 原误取 col0(水平水线)，会把水平线画在竖直管内导致垂直水流异常，此处改为与 N 同用 col75。
     */
    fun lineSrcRect(direction: Char, frame: Int): android.graphics.Rect {
        val fl = FRAME_LENGTH
        val row4 = 4 * TILE  // 300
        return when (direction) {
            'E' -> android.graphics.Rect(
                fl * (frame - 1), row4,
                fl * (frame - 1) + fl, row4 + TILE
            )
            'W' -> android.graphics.Rect(
                TILE - fl * frame, row4,
                TILE - fl * frame + fl, row4 + TILE
            )
            'S' -> android.graphics.Rect(
                TILE, row4 + fl * (frame - 1),
                TILE + TILE, row4 + fl * (frame - 1) + fl
            )
            'N' -> android.graphics.Rect(
                TILE, row4 + TILE - fl * frame,
                TILE + TILE, row4 + TILE - fl * frame + fl
            )
            else -> android.graphics.Rect(0, row4, fl, row4 + TILE)
        }
    }

    /**
     * SubArcAnimo 源矩形 — 完全对应 M8 SubArcAnimo 函数。
     *
     * 弧形大小: 55×55, 右侧弧 x 有 +20 偏移, 底部弧 y 也有 +20 偏移
     * (弧形在图集行中的实际位置：顶部弧贴行上沿 offset=0，底部弧贴行下沿 offset=20)。
     *
     * LD: src=(75*(frame-1), 20),          size=55×55
     * DL: src=(75*(15-frame), 20),         size=55×55
     * LU: src=(75*(15-frame), 150),        size=55×55
     * UL: src=(75*(frame-1), 150),         size=55×55
     * UR: src=(75*(15-frame)+20, 75),      size=55×55
     * RU: src=(75*(frame-1)+20, 75),       size=55×55
     * RD: src=(75*(15-frame)+20, 245),     size=55×55
     * DR: src=(75*(frame-1)+20, 245),      size=55×55
     */
    fun arcSrcRect(from: Char, to: Char, frame: Int): android.graphics.Rect {
        val arcSize = ARC_IMG  // 55
        val revFrame = ANIM_FRAMES + 1 - frame  // 16 - frame

        return when {
            // 底部弯弧（L→D / D→L）：弧形实际位于图集行的下沿，需向下偏移 ARC_DESC(20)，
            // 否则只截取到该行上方空白，导致水流图坐标错位。
            (from == 'L' && to == 'D') ->
                android.graphics.Rect(TILE * (frame - 1), ARC_DESC, TILE * (frame - 1) + arcSize, ARC_DESC + arcSize)
            (from == 'D' && to == 'L') ->
                android.graphics.Rect(TILE * (revFrame - 1), ARC_DESC, TILE * (revFrame - 1) + arcSize, ARC_DESC + arcSize)
            // 顶部左弧（L→U / U→L）：弧形位于图集行上沿，偏移 0。
            (from == 'L' && to == 'U') ->
                android.graphics.Rect(TILE * (revFrame - 1), 2 * TILE, TILE * (revFrame - 1) + arcSize, 2 * TILE + arcSize)
            (from == 'U' && to == 'L') ->
                android.graphics.Rect(TILE * (frame - 1), 2 * TILE, TILE * (frame - 1) + arcSize, 2 * TILE + arcSize)
            // 顶部右弧（U→R / R→U）：弧形位于图集行上沿，x 偏移 +20。
            (from == 'U' && to == 'R') ->
                android.graphics.Rect(TILE * (revFrame - 1) + ARC_DESC, TILE, TILE * (revFrame - 1) + ARC_DESC + arcSize, TILE + arcSize)
            (from == 'R' && to == 'U') ->
                android.graphics.Rect(TILE * (frame - 1) + ARC_DESC, TILE, TILE * (frame - 1) + ARC_DESC + arcSize, TILE + arcSize)
            // 底部右弧（R→D / D→R）：弧形位于图集行下沿，x 偏移 +20，y 也需偏移 +20。
            (from == 'R' && to == 'D') ->
                android.graphics.Rect(TILE * (revFrame - 1) + ARC_DESC, 3 * TILE + ARC_DESC, TILE * (revFrame - 1) + ARC_DESC + arcSize, 3 * TILE + ARC_DESC + arcSize)
            (from == 'D' && to == 'R') ->
                android.graphics.Rect(TILE * (frame - 1) + ARC_DESC, 3 * TILE + ARC_DESC, TILE * (frame - 1) + ARC_DESC + arcSize, 3 * TILE + ARC_DESC + arcSize)
            else -> android.graphics.Rect(0, 0, arcSize, arcSize)
        }
    }
}
