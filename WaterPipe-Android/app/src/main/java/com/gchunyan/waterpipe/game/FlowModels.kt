package com.gchunyan.waterpipe.game

/** 溢出描述：某方块某方向开口无衔接。*/
data class ErrBox(val box: Int, val dir: Char)

/** 水流动画一步：某方块入口 -> 某波推进过程，用于 UI 局部重绘进度。*/
data class FlowStep(
    val box: Int,
    val from: Char,        // 入水口
    val to: Char,          // 出水口 (与 from 相同表示单端)
    val frame: Int         // 当前帧 0..ANIM_FRAMES-1
)
