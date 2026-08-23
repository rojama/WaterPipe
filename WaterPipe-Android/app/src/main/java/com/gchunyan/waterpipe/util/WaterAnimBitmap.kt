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
     * 原 M8 代码：
     *   'E'/'W' 使用 row 4 第一个 tile (x=0..75) 的水平水纹
     *   'S'/'N' 使用 row 4 第二个 tile (x=75..150) 的垂直水纹
     * frame: 1..15
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
     * 计算 SubArcAnimo 的源矩形。
     * 原 M8 弧形行映射：
     *   Row 0 (y=0):   LD / DL
     *   Row 1 (y=75):  UR / RU
     *   Row 2 (y=150): LU / UL
     *   Row 3 (y=225): RD / DR
     * 某些方向组合使用反向帧 (15-frame)。
     * arc 区域: x = TILE*(frame-1)+17 .. +41, 宽24; y = row+17 .. row+41, 高24
     */
    fun arcSrcRect(from: Char, to: Char, frame: Int): android.graphics.Rect {
        val a1 = 17
        val a2 = 41
        val arcW = a2 - a1  // 24

        // 确定行 + 是否反向帧
        val y: Int
        val actualFrame: Int
        when {
            (from == 'L' && to == 'D') -> { y = 0;        actualFrame = frame }
            (from == 'D' && to == 'L') -> { y = 0;        actualFrame = 16 - frame }  // 反向
            (from == 'U' && to == 'R') -> { y = TILE;     actualFrame = 16 - frame }  // 反向
            (from == 'R' && to == 'U') -> { y = TILE;     actualFrame = frame }
            (from == 'L' && to == 'U') -> { y = 2 * TILE; actualFrame = 16 - frame }  // 反向
            (from == 'U' && to == 'L') -> { y = 2 * TILE; actualFrame = frame }
            (from == 'R' && to == 'D') -> { y = 3 * TILE; actualFrame = 16 - frame }  // 反向
            (from == 'D' && to == 'R') -> { y = 3 * TILE; actualFrame = frame }
            else -> { y = 0; actualFrame = frame }
        }
        return android.graphics.Rect(
            TILE * (actualFrame - 1) + a1, y + a1,
            TILE * (actualFrame - 1) + a2, y + a2
        )
    }
}
