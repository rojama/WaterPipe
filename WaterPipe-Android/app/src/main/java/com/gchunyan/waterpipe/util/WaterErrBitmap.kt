package com.gchunyan.waterpipe.util

import android.content.Context
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.graphics.Canvas
import android.graphics.ColorMatrix
import android.graphics.ColorMatrixColorFilter
import android.graphics.Paint
import android.graphics.PorterDuff
import android.graphics.PorterDuffXfermode
import com.gchunyan.waterpipe.R

/**
 * 水溢出动画图集（droplets24bit.bmp）辅助，完全还原原版 ErrAnimo()。
 *
 * 原图 328×1384：每帧高 80、行距 81（第 f 帧顶部 = f*81+1），共 17 帧(0..16)。
 * 每帧横向 4 个 80px tile（x=1/82/163/244，间距 1px）：
 *   左水滴 = mask(x=1) + color(x=82)；右水滴 = mask(x=163) + color(x=244)。
 *
 * 原版用 GDI 位运算 (dst AND mask) OR color 绘制；此处把 (mask,color) 预合成成
 * 透明 ARGB：以 mask 亮度作 alpha，对 color 取色，得到可直接 alpha 混合的水滴图。
 */
object WaterErrBitmap {

    /** 帧数（i=0..16）。 */
    const val FRAMES = 17
    /** 帧高度。 */
    const val FRAME_SIZE = 80
    /** 帧垂直间距。 */
    const val PITCH = 81
    /** 首行帧顶部偏移。 */
    const val TOP_OFFSET = 1

    private var source: Bitmap? = null
    private var frames: Array<Array<Bitmap?>> = emptyArray() // [frame][0=左,1=右]

    fun ensureLoaded(ctx: Context) {
        if (source != null) return
        runCatching {
            val options = BitmapFactory.Options().apply { inPreferredConfig = Bitmap.Config.ARGB_8888 }
            source = BitmapFactory.decodeResource(ctx.resources, R.drawable.droplets24bit, options)
            buildFrames()
        }
    }

    fun isLoaded(): Boolean = frames.isNotEmpty()

    /** 第 f 帧的水滴图（left=true 取左侧水滴）。 */
    fun frame(f: Int, left: Boolean): Bitmap? {
        val a = frames.getOrNull(f) ?: return null
        return a[if (left) 0 else 1]
    }

    fun recycle() {
        for (row in frames) for (b in row) b?.recycle()
        frames = emptyArray()
        source?.recycle(); source = null
    }

    private fun buildFrames() {
        val src = source ?: return
        frames = Array(FRAMES) { arrayOfNulls<Bitmap>(2) }
        // 帧顶部按 原版 i*81+1（BitmapFactory 已转 top-down）
        for (f in 0 until FRAMES) {
            val top = TOP_OFFSET + PITCH * f
            if (top + FRAME_SIZE > src.height) break
            frames[f][0] = buildDroplet(src, 1, top)
            frames[f][1] = buildDroplet(src, 163, top)
        }
    }

    /** 从 maskTile=colorTile 相邻 81 偏移的两 tile 合成透明水滴。 */
    private fun buildDroplet(src: Bitmap, maskLeft: Int, top: Int): Bitmap {
        val colorLeft = maskLeft + 81 // 82 或 244
        val out = Bitmap.createBitmap(FRAME_SIZE, FRAME_SIZE, Bitmap.Config.ARGB_8888)
        val canvas = Canvas(out)
        val paint = Paint(Paint.FILTER_BITMAP_FLAG)
        val rect = android.graphics.Rect(0, 0, FRAME_SIZE, FRAME_SIZE)
        // 1) 画 color tile（不透明）
        canvas.drawBitmap(src, android.graphics.Rect(colorLeft, top, colorLeft + FRAME_SIZE, top + FRAME_SIZE),
            rect, paint)
        // 2) 以 mask 亮度作 alpha，去掉非水滴背景
        val cm = ColorMatrix().apply {
            set(floatArrayOf(
                1f, 0f, 0f, 0f, 0f,
                0f, 1f, 0f, 0f, 0f,
                0f, 0f, 1f, 0f, 0f,
                0.299f, 0.587f, 0.114f, 0f, 0f
            ))
        }
        paint.colorFilter = ColorMatrixColorFilter(cm)
        paint.xfermode = PorterDuffXfermode(PorterDuff.Mode.DST_IN)
        canvas.drawBitmap(src, android.graphics.Rect(maskLeft, top, maskLeft + FRAME_SIZE, top + FRAME_SIZE),
            rect, paint)
        return out
    }
}