package com.gchunyan.waterpipe.data

/** 排行榜条目：名次=index+1；附 bitmap 文件名（应用内 files/ranking/ 目录下相对路径）。*/
data class RankingEntry(
    val name: String,
    val score: Int,
    val screenshotFile: String? = null
) {
    companion object {
        const val MAX_SIZE = 20
    }
}

/** 应用偏好设置。*/
data class AppSettings(
    val volume: Int = 100,            // 0..100 (替代原 0..0xFFFF)
    val soundsOn: Boolean = true,
    val isDebug: Boolean = false,
    val infoPanelLeft: Boolean = true,   // isPreLeft
    val queueDirectionUp: Boolean = true, // isPreListUp
    val homeAction: HomeAction = HomeAction.CLOSE,
    val lastPlayerName: String = "新玩家"
)

enum class HomeAction(val value: Int) {
    INVALID(0), MINIMIZE(1), CLOSE(2);
    companion object {
        fun of(i: Int): HomeAction = values().firstOrNull { it.value == i } ?: CLOSE
    }
}
