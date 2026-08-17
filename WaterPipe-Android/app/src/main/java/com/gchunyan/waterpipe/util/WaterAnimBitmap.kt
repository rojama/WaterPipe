package com.gchunyan.waterpipe.util

import android.content.Context
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import com.gchunyan.waterpipe.R

/**
 * AllWater.png 流水动画图集辅助。
 *
 * 原图 1125×375 = (15 × 75) × (5 × 75)。
 * Row 4 (y=300..375) 存"直线水纹理"（垂直水柱截面）。
 * Row 0..3 存 4 种方向弯管的弧形水纹理。
 *
 * SubLineAnimo 用一条 frame_length=5px 的窄带在 row 4 上横向/纵向滑动，
 * 对应 15 帧把水"扫"过整格。SubArcAnimo 用同样方式在 row 0..3 上扫过 75×75 的弧形区。
 */
object WaterAnimBitmap {

    private const val TILE = 75
    private const val TOTAL_COLS = 15
    private const val TOTAL_ROWS = 5
    private const val FRAME_LENGTH = 5

    private var atlas: Bitmap? = null

    /** 初始化（可反复调用，幂等）。在后台线程解码。*/
    fun ensureLoaded(ctx: Context) {
        if (atlas != null) return
        runCatching {
            val options = BitmapFactory.Options().apply { inPreferredConfig = Bitmap.Config.ARGB_8888 }
            atlas = BitmapFactory.decodeResource(ctx.resources, R.drawable.allwater, options)
        }
    }

    fun isLoaded(): Boolean = atlas != null

    fun get(): Bitmap? = atlas

    fun recycle() {
        atlas?.recycle()
        atlas = null
    }

    /**
     * 计算 SubLineAnimo 在 AllWater 图上的源矩形。
     * 方向: 'E' 水从左入向右流; 'W' 水从右入向左流; 'S' 水从上入向下流; 'N' 水从下入向上流。
     * frame: 1..15（与原 C++ 相同）。
     */
    fun lineSrcRect(direction: Char, frame: Int): android.graphics.Rect {
        val fl = FRAME_LENGTH
        val row4 = TOTAL_ROWS * TILE - TILE  // 300
        return when (direction) {
            'W' -> android.graphics.Rect(
                TILE - fl * frame, row4,
                TILE - fl * frame + fl, row4 + TILE
            )
            'E' -> android.graphics.Rect(
                fl * (frame - 1), row4,
                fl * (frame - 1) + fl, row4 + TILE
            )
            'S' -> android.graphics.Rect(
                0, row4 + fl * (frame - 1),
                TILE, row4 + fl * (frame - 1) + fl
            )
            'N' -> android.graphics.Rect(
                0, row4 + TILE - fl * frame,
                TILE, row4 + TILE - fl * frame + fl
            )
            else -> android.graphics.Rect(0, row4, fl, row4 + TILE)
        }
    }

    /**
     * 计算 SubArcAnimo 的源矩形。
     * from/to 是入口/出口端口字母 'L','R','U','D'。
     * frame: 1..15。
     */
    fun arcSrcRect(from: Char, to: Char, frame: Int): android.graphics.Rect {
        val a1 = 17
        val a2 = 41
        // 枚举所有 L<->R/U<->D 组合
        val y = when {
            (from == 'L' && to == 'D') || (from == 'D' && to == 'L') -> 0           // row 0: y=0
            (from == 'L' && to == 'U') || (from == 'U' && to == 'L') -> 2 * TILE   // row 2: y=150
            (from == 'U' && to == 'R') || (from == 'R' && to == 'U') -> 2 * TILE   // row 2: y=150 (same texture)
            (from == 'R' && to == 'D') || (from == 'D' && to == 'R') -> 3 * TILE   // row 3: y=225
            else -> 0
        }
        val a1_y = y + a1
        val a2_y = y + a2
        return android.graphics.Rect(
            TILE * (frame - 1) + a1, a1_y,
            TILE * (frame - 1) + a2, a2_y
        )
    }
}
