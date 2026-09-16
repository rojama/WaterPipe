package com.gchunyan.waterpipe.util

import android.content.Context
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.graphics.Canvas
import android.graphics.Paint
import android.graphics.PorterDuff
import android.graphics.PorterDuffXfermode
import android.graphics.Rect
import android.graphics.RectF
import android.net.Uri
import android.os.Build
import android.os.Environment
import com.gchunyan.waterpipe.R
import com.gchunyan.waterpipe.game.PipeTypes
import com.gchunyan.waterpipe.game.WaterPipeEngine
import java.io.File
import java.io.FileOutputStream
import kotlin.math.roundToInt

/** 把 canvas/view 绘制成 Bitmap 并保存为 PNG。
 * （修复）原 SaveBmp 对 CreateDIBSection 位指针做 `delete`，这里使用 Bitmap.compress 正确释放。 */
object BitmapUtil {

    /** 把 CanvasView 绘制内容保存为 PNG。return 文件绝对路径。*/
    fun saveBitmapAsPng(context: Context, bitmap: Bitmap, file: File): String {
        file.parentFile?.mkdirs()
        FileOutputStream(file).use { fos ->
            bitmap.compress(Bitmap.CompressFormat.PNG, 95, fos)
            fos.flush()
        }
        return file.absolutePath
    }

    /** 截取游戏画面。传入一个可执行的绘制 lambda。 */
    fun capture(width: Int, height: Int, draw: (Canvas) -> Unit): Bitmap {
        val bm = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888)
        val c = Canvas(bm)
        val paint = Paint(Paint.ANTI_ALIAS_FLAG)
        paint.xfermode = PorterDuffXfermode(PorterDuff.Mode.SRC)
        c.drawARGB(255, 240, 248, 255)
        draw(c)
        return bm
    }

    /** 终局棋盘渲染为 PNG：浅色底 + wangge 网格 + 每格已放置管道。返回文件绝对路径。 */
    fun renderBoardPng(context: Context, engine: WaterPipeEngine, file: File): String {
        val cols = PipeTypes.COLS
        val rows = PipeTypes.ROWS
        val cell = 120
        val width = cols * cell
        val height = rows * cell
        val bm = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888)
        val canvas = Canvas(bm)
        val paint = Paint(Paint.ANTI_ALIAS_FLAG or Paint.FILTER_BITMAP_FLAG)
        canvas.drawColor(0xFFF0F8FF.toInt())
        BitmapFactory.decodeResource(context.resources, R.drawable.wangge)?.let { bg ->
            canvas.drawBitmap(bg, null, RectF(0f, 0f, width.toFloat(), height.toFloat()), paint)
        }
        val pad = cell * 0.08f
        for (r in 0 until rows) {
            for (c in 0 until cols) {
                val tag = engine.boxes[PipeTypes.boxIndex(r, c)].tag
                val name = PipeTypes.drawableNameFor(tag) ?: continue
                val id = context.resources.getIdentifier(name, "drawable", context.packageName)
                if (id == 0) continue
                BitmapFactory.decodeResource(context.resources, id)?.let { pipe ->
                    val dst = RectF(
                        c * cell + pad, r * cell + pad,
                        (c + 1) * cell - pad, (r + 1) * cell - pad
                    )
                    canvas.drawBitmap(pipe, null, dst, paint)
                }
            }
        }
        return saveBitmapAsPng(context, bm, file)
    }
}
