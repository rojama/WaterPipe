package com.gchunyan.waterpipe.util

import android.content.Context
import android.media.AudioManager
import android.media.SoundPool
import com.gchunyan.waterpipe.R
import java.util.EnumMap

/**
 * 音效播放器：SoundPool 替代原 PlaySound。
 * （修复）原代码 PlaySound 参数 IDR_WAVE_water_flow 实际指向 klunk.wav（标签错配），这里重新明确定义。
 */
class SoundManager(private val context: Context) {

    private val am: AudioManager = context.getSystemService(Context.AUDIO_SERVICE) as AudioManager
    private val pool: SoundPool = SoundPool.Builder()
        .setMaxStreams(4)
        .build()

    enum class Sfx(val resId: Int) {
        PLACE(R.raw.boink),                    // 放管到空格（原 boink）
        MERGE(R.raw.klunk),                     // 立交桥合并（原 klunk）
        BREAK(R.raw.glass_breaking),            // 管道替换/打破立交（原 glass_breaking）
        WARNING(R.raw.warning),                 // 溢出报警
        WATER(R.raw.water_flow),                // 水流循环
        TAP(R.raw.button)                       // 按钮点击
    }

    private val sfxIds: MutableMap<Sfx, Int> = EnumMap(Sfx::class.java)
    private var loadedCount = 0

    init {
        for (sfx in Sfx.values()) {
            pool.load(context, sfx.resId, 1)
        }
        pool.setOnLoadCompleteListener { _, _, _ -> loadedCount++ }
    }

    /** 播放一次。volumeScale 0..1；soundsOn=false 时不播放。*/
    fun play(sfx: Sfx, volumeScale: Float = 1f, loop: Boolean = false) {
        if (loadedCount < Sfx.values().size) return
        val sid = sfxIds.getOrPut(sfx) { pool.load(context, sfx.resId, 1) }
        val actual = (volumeScale.coerceIn(0f, 1f)) * (am.getStreamVolume(AudioManager.STREAM_MUSIC).toFloat() /
                am.getStreamMaxVolume(AudioManager.STREAM_MUSIC).toFloat().coerceAtLeast(1f))
        pool.play(sid, actual, actual, 1, if (loop) -1 else 0, 1f)
    }

    fun stopAll() { pool.autoPause() }

    fun release() { pool.release() }
}
