package com.gchunyan.waterpipe.game

/**
 * 每格状态。
 *
 * （修复）原 C++ 代码用 `in_lab` 拼接字符串，可能发生重复添加同一入口的问题；
 * 这里改用 Set<Char> 存储，天然去重，避免 GetNext 推导出重复项。
 */
data class BoxState(
    var tag: String = PipeTypes.EMPTY,
    val inLab: MutableSet<Char> = mutableSetOf(),
    val fullLab: MutableSet<Char> = mutableSetOf()
) {
    fun inLabAsString(): String = inLab.sorted().joinToString("")
    fun containsIn(c: Char): Boolean = inLab.contains(c)
    fun addIn(c: Char) { inLab.add(c) }
}
