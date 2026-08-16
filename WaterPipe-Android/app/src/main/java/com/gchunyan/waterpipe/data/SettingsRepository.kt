package com.gchunyan.waterpipe.data

import android.content.Context
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.intPreferencesKey
import androidx.datastore.preferences.core.booleanPreferencesKey
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.preferencesDataStore
import com.gchunyan.waterpipe.game.PipeTypes
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.map
import java.io.File

/**
 * 持久化仓库：DataStore 替代原 INI；files/ranking/ 存截图 PNG。
 *
 * （修复）原 app.ini 中 SaveSetting 会把用户在"系统音量"滑块上的值写回 dwOrgVolume，
 * 导致退出时无法恢复系统音量——这里把"系统音量"拆分成只读系统 API，
 * settings.volume 只表示游戏相对音量，不触摸全局系统音量。
 */
class SettingsRepository(context: Context) {

    private val ctx = context.applicationContext
    private val Context.dataStore by preferencesDataStore(name = "waterpipe_prefs")

    private object Keys {
        val volume = intPreferencesKey("volume")
        val soundsOn = booleanPreferencesKey("sounds_on")
        val isDebug = booleanPreferencesKey("is_debug")
        val infoPanelLeft = booleanPreferencesKey("info_panel_left")
        val queueUp = booleanPreferencesKey("queue_up")
        val homeAction = intPreferencesKey("home_action")
        val lastName = stringPreferencesKey("last_name")
    }

    val settingsFlow: Flow<AppSettings> = ctx.dataStore.data.map { p ->
        AppSettings(
            volume = (p[Keys.volume] ?: 100).coerceIn(0, 100),
            soundsOn = p[Keys.soundsOn] ?: true,
            isDebug = p[Keys.isDebug] ?: false,
            infoPanelLeft = p[Keys.infoPanelLeft] ?: true,
            queueDirectionUp = p[Keys.queueUp] ?: true,
            homeAction = HomeAction.of(p[Keys.homeAction] ?: HomeAction.CLOSE.value),
            lastPlayerName = p[Keys.lastName] ?: "新玩家"
        )
    }

    suspend fun get(): AppSettings = settingsFlow.first()

    suspend fun save(s: AppSettings) {
        ctx.dataStore.edit { p ->
            p[Keys.volume] = s.volume.coerceIn(0, 100)
            p[Keys.soundsOn] = s.soundsOn
            p[Keys.isDebug] = s.isDebug
            p[Keys.infoPanelLeft] = s.infoPanelLeft
            p[Keys.queueUp] = s.queueDirectionUp
            p[Keys.homeAction] = s.homeAction.value
            p[Keys.lastName] = s.lastPlayerName
        }
    }

    suspend fun saveVolume(volume: Int) {
        ctx.dataStore.edit { it[Keys.volume] = volume.coerceIn(0, 100) }
    }

    suspend fun saveSounds(on: Boolean) {
        ctx.dataStore.edit { it[Keys.soundsOn] = on }
    }

    suspend fun saveLastPlayerName(name: String) {
        ctx.dataStore.edit { it[Keys.lastName] = name }
    }

    // ---------- 排行榜 ----------

    fun rankingDir(): File = File(ctx.filesDir, "ranking").apply { mkdirs() }

    suspend fun loadRanking(): List<RankingEntry> {
        val list = mutableListOf<RankingEntry>()
        val prefs = ctx.dataStore.data.first()
        for (i in 1..RankingEntry.MAX_SIZE) {
            val scoreKey = intPreferencesKey("ranking_score_$i")
            val nameKey = stringPreferencesKey("ranking_name_$i")
            val score = prefs[scoreKey] ?: break
            val name = prefs[nameKey] ?: break
            val file = File(rankingDir(), "rank_$i.png")
            list.add(RankingEntry(name, score, if (file.exists()) file.name else null))
        }
        return list
    }

    suspend fun insertRanking(entry: RankingEntry, rankNo: Int): List<RankingEntry> {
        val current = loadRanking().toMutableList()
        current.add(rankNo - 1, entry)
        while (current.size > RankingEntry.MAX_SIZE) current.removeLast()

        // 1. 重命名老截图文件：从后往前，N→N+1，末尾删除
        val dir = rankingDir()
        for (i in current.size downTo rankNo) {
            val oldF = File(dir, "rank_$i.png")
            val newF = File(dir, "rank_${i + 1}.png")
            if (i == RankingEntry.MAX_SIZE && newF.exists()) newF.delete()
            if (oldF.exists()) oldF.renameTo(newF)
        }
        // 2. 若新条目附带了临时 PNG 文件，从缓存位置移动到 rank_rankNo
        entry.screenshotFile?.let { tempPath ->
            val src = File(tempPath)
            if (src.exists()) {
                val dst = File(dir, "rank_$rankNo.png")
                if (dst.exists()) dst.delete()
                src.copyTo(dst, overwrite = true)
                src.delete()
            }
        }

        // 3. 写 DataStore：从 rankNo 开始覆盖 name_N / score_N
        ctx.dataStore.edit { p ->
            for ((idx, e) in current.withIndex()) {
                val pos = idx + 1
                p[intPreferencesKey("ranking_score_$pos")] = e.score
                p[stringPreferencesKey("ranking_name_$pos")] = e.name
            }
            // 超过后清理：当前不足 20 时，后续旧 key 可能还在，故清到 MAX_SIZE
            for (pos in (current.size + 1)..RankingEntry.MAX_SIZE) {
                p.remove(intPreferencesKey("ranking_score_$pos"))
                p.remove(stringPreferencesKey("ranking_name_$pos"))
            }
        }
        return current
    }

    /** 返回分数应该插入的名次（1-based），若未进榜返回 -1。 */
    fun getRankingNo(currentList: List<RankingEntry>, score: Int): Int {
        if (score <= 0) return -1
        if (currentList.isEmpty()) return 1
        for ((idx, e) in currentList.withIndex()) {
            if (score > e.score) return idx + 1
        }
        return if (currentList.size < RankingEntry.MAX_SIZE) currentList.size + 1 else -1
    }

    fun screenshotFileFor(rankNo: Int): File = File(rankingDir(), "rank_$rankNo.png")
    fun tempScreenshotFile(): File = File(ctx.cacheDir, "ranking_tmp_${System.currentTimeMillis()}.png")
}
