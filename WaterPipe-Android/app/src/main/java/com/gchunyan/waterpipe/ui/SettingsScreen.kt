package com.gchunyan.waterpipe.ui

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.CenterAlignedTopAppBar
import androidx.compose.material3.Divider
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Switch
import androidx.compose.material3.Text
import androidx.compose.material3.TopAppBarDefaults
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.gchunyan.waterpipe.data.AppSettings
import com.gchunyan.waterpipe.data.HomeAction

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SettingsScreen(
    vm: GameViewModel,
    onBack: () -> Unit,
    onReadme: () -> Unit,
    onAbout: () -> Unit
) {
    val s by vm.settings.collectAsState()
    var draft by remember(s) { mutableStateOf(s) }

    Scaffold(
        topBar = {
            CenterAlignedTopAppBar(
                title = { Text("设置") },
                navigationIcon = { IconButton(onClick = onBack) { Icon(Icons.Default.ArrowBack, null) } },
                actions = {
                    IconButton(onClick = { vm.saveSettings(draft); onBack() }) {
                        Text("保存", color = MaterialTheme.colorScheme.primary)
                    }
                },
                colors = TopAppBarDefaults.centerAlignedTopAppBarColors(
                    containerColor = MaterialTheme.colorScheme.primaryContainer
                )
            )
        }
    ) { pad ->
        Column(
            Modifier
                .padding(pad)
                .fillMaxSize()
                .verticalScroll(rememberScrollState())
        ) {
            SectionTitle("界面")
            LineChoice("信息面板位置",
                options = listOf("左边", "右边"),
                selected = if (draft.infoPanelLeft) "左边" else "右边",
                onSelect = { draft = draft.copy(infoPanelLeft = it == "左边") })
            LineChoice("管道队列方向",
                options = listOf("向上", "向下"),
                selected = if (draft.queueDirectionUp) "向上" else "向下",
                onSelect = { draft = draft.copy(queueDirectionUp = it == "向上") })

            SectionTitle("声音")
            SliderLine("游戏音量 (${draft.volume})", 0..100, draft.volume,
                onValue = { v -> draft = draft.copy(volume = v) })
            SwitchLine("音效", enabled = draft.soundsOn, onChange = { b -> draft = draft.copy(soundsOn = b) })

            SectionTitle("其它")
            LineChoice("返回键动作",
                options = listOf("无效", "最小化", "关闭"),
                selected = when (draft.homeAction) {
                    HomeAction.INVALID -> "无效"
                    HomeAction.MINIMIZE -> "最小化"
                    HomeAction.CLOSE -> "关闭"
                },
                onSelect = { v ->
                    val act = when (v) {
                        "无效" -> HomeAction.INVALID
                        "最小化" -> HomeAction.MINIMIZE
                        else -> HomeAction.CLOSE
                    }
                    draft = draft.copy(homeAction = act)
                })
            NavLine("游戏说明", onClick = onReadme)
            NavLine("关于游戏", onClick = onAbout)

            Spacer(Modifier.height(24.dp))
            Text(
                "提示：Android 版已去除原 M8 版的 DRM 机器码/注册码校验，可直接使用。",
                modifier = Modifier.padding(16.dp, 8.dp),
                fontSize = 13.sp,
                color = MaterialTheme.colorScheme.onSurfaceVariant
            )

            Spacer(Modifier.height(24.dp))
            Row(Modifier.fillMaxWidth().padding(horizontal = 16.dp),
                horizontalArrangement = Arrangement.spacedBy(12.dp)) {
                Button(modifier = Modifier.weight(1f), onClick = onBack) { Text("取消") }
                Button(modifier = Modifier.weight(1f), onClick = { vm.saveSettings(draft); onBack() }) { Text("保存") }
            }
        }
    }
}

@Composable
private fun SectionTitle(title: String) {
    Text(title,
        modifier = Modifier.padding(16.dp, 12.dp, 8.dp, 4.dp),
        color = MaterialTheme.colorScheme.primary,
        fontSize = 14.sp
    )
    Divider(color = MaterialTheme.colorScheme.outlineVariant)
}

@Composable
private fun LineChoice(title: String, options: List<String>, selected: String, onSelect: (String) -> Unit) {
    Card(
        Modifier.fillMaxWidth().padding(8.dp),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surfaceVariant)
    ) {
        Column(Modifier.padding(12.dp, 8.dp)) {
            Text(title, fontSize = 14.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
            Spacer(Modifier.height(8.dp))
            Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                for (opt in options) {
                    val sel = opt == selected
                    Button(
                        onClick = { onSelect(opt) },
                        modifier = Modifier.weight(1f)
                    ) {
                        Text(opt, color = if (sel) MaterialTheme.colorScheme.onPrimary else MaterialTheme.colorScheme.primary)
                    }
                }
            }
        }
    }
}

@Composable
private fun SwitchLine(title: String, enabled: Boolean, onChange: (Boolean) -> Unit) {
    Card(
        Modifier.fillMaxWidth().padding(8.dp),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surfaceVariant)
    ) {
        Row(Modifier.fillMaxWidth().padding(16.dp, 8.dp), verticalAlignment = Alignment.CenterVertically) {
            Text(title, Modifier.weight(1f))
            Switch(checked = enabled, onCheckedChange = onChange)
        }
    }
}

@Composable
private fun SliderLine(title: String, range: IntRange, value: Int, onValue: (Int) -> Unit) {
    Card(
        Modifier.fillMaxWidth().padding(8.dp),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surfaceVariant)
    ) {
        Column(Modifier.padding(16.dp, 8.dp)) {
            Text(title, fontSize = 14.sp)
            androidx.compose.material3.Slider(
                value = value.toFloat(),
                valueRange = range.first.toFloat()..range.last.toFloat(),
                steps = (range.last - range.first - 1),
                onValueChange = { onValue(it.toInt()) }
            )
        }
    }
}

@Composable
private fun NavLine(title: String, onClick: () -> Unit) {
    Card(
        Modifier.fillMaxWidth().padding(8.dp).clickable(onClick = onClick),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface)
    ) {
        Row(
            Modifier.fillMaxWidth().padding(16.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Text(title, Modifier.weight(1f))
            Icon(Icons.Default.ArrowBack, contentDescription = null) // 占位
        }
    }
}
