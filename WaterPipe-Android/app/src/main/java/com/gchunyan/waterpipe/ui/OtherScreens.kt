package com.gchunyan.waterpipe.ui

import android.net.Uri
import androidx.compose.foundation.Image
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.CenterAlignedTopAppBar
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.material3.TopAppBarDefaults
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.gchunyan.waterpipe.R
import com.gchunyan.waterpipe.data.RankingEntry
import com.gchunyan.waterpipe.data.SettingsRepository
import com.gchunyan.waterpipe.game.PipeTypes
import kotlinx.coroutines.flow.collect

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun RankingScreen(
    vm: GameViewModel,
    highlightRank: Int,
    showNewGame: Boolean,
    onBack: () -> Unit,
    onNewGame: () -> Unit,
    onOpenScreenshot: (rank: Int) -> Unit
) {
    LaunchedEffect(Unit) { vm.refreshRanking() }
    val list by vm.ranking.collectAsState()

    Scaffold(
        topBar = {
            CenterAlignedTopAppBar(
                title = { Text(stringResource(R.string.btn_ranking)) },
                navigationIcon = { IconButton(onClick = onBack) { Icon(Icons.Default.ArrowBack, null) } },
                actions = {
                    if (showNewGame) TextButton(onClick = onNewGame) {
                        Text(stringResource(R.string.btn_new_game),
                            color = MaterialTheme.colorScheme.onPrimaryContainer)
                    }
                },
                colors = TopAppBarDefaults.centerAlignedTopAppBarColors(
                    containerColor = MaterialTheme.colorScheme.primaryContainer
                )
            )
        },
        bottomBar = {
            Row(Modifier.fillMaxWidth().padding(12.dp), horizontalArrangement = Arrangement.spacedBy(12.dp)) {
                Button(onClick = onBack, Modifier.weight(1f)) { Text(stringResource(R.string.btn_return)) }
                if (showNewGame) {
                    Button(onClick = onNewGame, Modifier.weight(1f)) { Text(stringResource(R.string.btn_new_game)) }
                }
            }
        }
    ) { pad ->
        if (list.isEmpty()) {
            Box(Modifier.fillMaxSize().padding(pad)) {
                Text("还没有记录，快来挑战第一名吧！", Modifier.align(Alignment.Center))
            }
        } else {
            LazyColumn(Modifier.fillMaxSize().padding(pad)) {
                items(list, key = { "${it.name}_${it.score}_${it.screenshotFile}" }) { entry ->
                    val pos = list.indexOf(entry) + 1
                    val highlight = pos == highlightRank
                    Card(
                        Modifier
                            .fillMaxWidth()
                            .padding(8.dp)
                            .clickable { onOpenScreenshot(pos) },
                        colors = CardDefaults.cardColors(
                            containerColor = if (highlight) Color(0xFFFFF9C4)
                            else MaterialTheme.colorScheme.surfaceVariant
                        )
                    ) {
                        Row(
                            Modifier.fillMaxWidth().padding(16.dp, 12.dp),
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            Text(
                                text = "$pos.",
                                fontWeight = FontWeight.Bold,
                                fontSize = 22.sp,
                                modifier = Modifier.width(44.dp)
                            )
                            Column(Modifier.weight(1f)) {
                                Text(entry.name, fontSize = 18.sp, fontWeight = FontWeight.SemiBold)
                                Text(
                                    stringResource(R.string.ranking_item_format, pos, entry.score, entry.name)
                                        .substringAfter(entry.name).let { "${entry.score} 分" },
                                    fontSize = 14.sp,
                                    color = MaterialTheme.colorScheme.onSurfaceVariant
                                )
                            }
                            Text(">", fontSize = 24.sp, color = MaterialTheme.colorScheme.outline)
                        }
                    }
                }
            }
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ReadmeScreen(onBack: () -> Unit) {
    Scaffold(
        topBar = {
            CenterAlignedTopAppBar(
                title = { Text(stringResource(R.string.readme_title)) },
                navigationIcon = { IconButton(onClick = onBack) { Icon(Icons.Default.ArrowBack, null) } },
                colors = TopAppBarDefaults.centerAlignedTopAppBarColors(
                    containerColor = MaterialTheme.colorScheme.primaryContainer
                )
            )
        }
    ) { pad ->
        Column(
            Modifier
                .fillMaxSize()
                .padding(pad)
                .verticalScroll(rememberScrollState())
                .padding(16.dp)
        ) {
            Text(stringResource(R.string.readme_text), fontSize = 16.sp, lineHeight = 26.sp)
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun AboutScreen(onBack: () -> Unit) {
    Scaffold(
        topBar = {
            CenterAlignedTopAppBar(
                title = { Text(stringResource(R.string.about_title)) },
                navigationIcon = { IconButton(onClick = onBack) { Icon(Icons.Default.ArrowBack, null) } },
                colors = TopAppBarDefaults.centerAlignedTopAppBarColors(
                    containerColor = MaterialTheme.colorScheme.primaryContainer
                )
            )
        }
    ) { pad ->
        Column(
            Modifier
                .fillMaxSize()
                .padding(pad)
                .verticalScroll(rememberScrollState())
                .padding(16.dp)
        ) {
            Text(stringResource(R.string.about_text), fontSize = 16.sp, lineHeight = 26.sp)
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun NameEditScreen(
    rank: Int,
    score: Int,
    defaultName: String,
    onCancel: () -> Unit,
    onConfirm: (String) -> Unit
) {
    var name by remember { mutableStateOf(defaultName) }
    AlertDialog(
        onDismissRequest = onCancel,
        title = { Text(stringResource(R.string.congrats_rank, rank, score)) },
        text = {
            Column {
                Text(stringResource(R.string.hint_name))
                Spacer(Modifier.height(8.dp))
                OutlinedTextField(value = name, onValueChange = { name = it.take(7) }, singleLine = true)
            }
        },
        confirmButton = { TextButton(onClick = { onConfirm(name.ifBlank { defaultName }) }) {
            Text(stringResource(R.string.btn_record))
        } },
        dismissButton = { TextButton(onClick = onCancel) {
            Text(stringResource(R.string.btn_no_record))
        } }
    )
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ScreenshotScreen(
    rank: Int,
    repo: SettingsRepository,
    onBack: () -> Unit
) {
    val ctx = LocalContext.current
    val file = remember(rank) { repo.screenshotFileFor(rank) }
    Scaffold(
        topBar = {
            CenterAlignedTopAppBar(
                title = { Text("第 $rank 名截图") },
                navigationIcon = { IconButton(onClick = onBack) { Icon(Icons.Default.ArrowBack, null) } },
                colors = TopAppBarDefaults.centerAlignedTopAppBarColors(
                    containerColor = MaterialTheme.colorScheme.primaryContainer
                )
            )
        }
    ) { pad ->
        Box(Modifier.fillMaxSize().padding(pad)) {
            if (file.exists()) {
                val bm = remember(file) {
                    android.graphics.BitmapFactory.decodeFile(file.absolutePath)?.asImageBitmap()
                }
                if (bm != null) {
                    Image(bitmap = bm, contentDescription = null,
                        modifier = Modifier.fillMaxSize(),
                        contentScale = ContentScale.Fit)
                } else {
                    Text(stringResource(R.string.screenshot_missing), Modifier.align(Alignment.Center))
                }
            } else {
                Text(stringResource(R.string.screenshot_missing), Modifier.align(Alignment.Center))
            }
        }
    }
}
