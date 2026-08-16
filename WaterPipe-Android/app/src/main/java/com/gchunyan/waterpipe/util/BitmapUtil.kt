package com.gchunyan.waterpipe.util

import android.content.Context
import android.graphics.Bitmap
import android.graphics.Canvas
import android.graphics.Paint
import android.graphics.PorterDuff
import android.graphics.PorterDuffXfermode
import android.graphics.Rect
import android.graphics.RectF
import android.net.Uri
import android.os.Build
import android.os.Environment
import com.gchunyan.waterpipe.game.PipeTypes
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
}
