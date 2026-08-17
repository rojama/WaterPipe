package com.gchunyan.waterpipe.ui

import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.platform.LocalContext
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewmodel.CreationExtras
import androidx.lifecycle.viewModelScope
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.gchunyan.waterpipe.data.AppSettings
import com.gchunyan.waterpipe.data.HomeAction
import com.gchunyan.waterpipe.data.RankingEntry
import com.gchunyan.waterpipe.data.SettingsRepository
import com.gchunyan.waterpipe.game.WaterPipeEngine
import com.gchunyan.waterpipe.util.SoundManager
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.launch

/**
 * 应用入口导航：游戏 / 设置 / 排行榜 / 说明 / 关于 / 姓名输入 / 截图查看。
 * （优化）使用 Jetpack Compose + Navigation，替代原模态 DoModal；水流动画用协程 + Ticker 实现，
 *  不再用 CreateThread + Sleep；避免原代码的线程数据泄露和 UI 线程跨线程操作 Bug。
 */
sealed class Screen(val route: String) {
    object Game : Screen("game")
    object Settings : Screen("settings")
    object Ranking : Screen("ranking?highlight={highlight}&showNew={showNew}") {
        fun build(highlight: Int = -1, showNew: Boolean = false): String =
            "ranking?highlight=$highlight&showNew=$showNew"
    }
    object Readme : Screen("readme")
    object About : Screen("about")
    object NameEdit : Screen("name?rank={rank}&score={score}") {
        fun build(rank: Int, score: Int): String = "name?rank=$rank&score=$score"
    }
    object Screenshot : Screen("ss/{rank}") {
        fun build(rank: Int): String = "ss/$rank"
    }
}

class GameViewModel(private val repo: SettingsRepository) : ViewModel() {

    val engine = WaterPipeEngine()

    var gameVersion by mutableIntStateOf(0)
        private set

    val settings: StateFlow<AppSettings> = repo.settingsFlow
        .stateIn(viewModelScope, SharingStarted.Eagerly, AppSettings())

    private val _ranking = MutableStateFlow<List<RankingEntry>>(emptyList())
    val ranking: StateFlow<List<RankingEntry>> = _ranking.asStateFlow()

    init {
        viewModelScope.launch {
            _ranking.value = repo.loadRanking()
            engine.newGame()
            gameVersion++
        }
    }

    fun notifyChanged() { gameVersion++ }

    fun newGame() {
        engine.newGame()
        gameVersion++
    }

    fun saveScreenshotAndInsert(tempFile: String, name: String, score: Int, rankNo: Int) {
        viewModelScope.launch(Dispatchers.IO) {
            val list = repo.insertRanking(
                RankingEntry(name = name, score = score, screenshotFile = tempFile), rankNo
            )
            repo.saveLastPlayerName(name)
            _ranking.value = list
        }
    }

    fun refreshRanking() {
        viewModelScope.launch { _ranking.value = repo.loadRanking() }
    }

    fun getRankingNo(score: Int): Int = repo.getRankingNo(_ranking.value, score)

    fun saveSettings(s: AppSettings) {
        viewModelScope.launch { repo.save(s) }
    }
}

@Composable
fun WaterPipeApp() {
    val ctx = LocalContext.current
    val repo = remember { SettingsRepository(ctx) }
    val sound = remember { SoundManager(ctx) }
    val nav = rememberNavController()
    val vm: GameViewModel = viewModel(factory = GenericFactory { GameViewModel(repo) })

    NavHost(navController = nav, startDestination = Screen.Game.route) {
        composable(Screen.Game.route) {
            GameScreen(vm, sound, nav)
        }
        composable(Screen.Settings.route) {
            SettingsScreen(vm, onBack = { nav.popBackStack() },
                onReadme = { nav.navigate(Screen.Readme.route) },
                onAbout = { nav.navigate(Screen.About.route) })
        }
        composable(Screen.Ranking.route) { entry ->
            val hl = entry.arguments?.getString("highlight")?.toIntOrNull() ?: -1
            val showNew = entry.arguments?.getString("showNew")?.toBoolean() ?: false
            RankingScreen(vm, highlightRank = hl, showNewGame = showNew,
                onBack = { nav.popBackStack() },
                onNewGame = { vm.newGame(); nav.popBackStack(Screen.Game.route, inclusive = false) },
                onOpenScreenshot = { nav.navigate(Screen.Screenshot.build(it)) })
        }
        composable(Screen.Readme.route) { ReadmeScreen(onBack = { nav.popBackStack() }) }
        composable(Screen.About.route) { AboutScreen(onBack = { nav.popBackStack() }) }
        composable(Screen.NameEdit.route) { entry ->
            val rank = entry.arguments?.getString("rank")?.toIntOrNull() ?: 0
            val score = entry.arguments?.getString("score")?.toIntOrNull() ?: 0
            NameEditScreen(
                rank = rank, score = score,
                defaultName = vm.settings.value.lastPlayerName,
                onCancel = { nav.previousBackStackEntry?.savedStateHandle?.set("name_result", false); nav.popBackStack() },
                onConfirm = { name ->
                    nav.previousBackStackEntry?.savedStateHandle?.set("name_result", true)
                    nav.previousBackStackEntry?.savedStateHandle?.set("name_value", name)
                    nav.popBackStack()
                }
            )
        }
        composable(Screen.Screenshot.route) { entry ->
            val rank = entry.arguments?.getString("rank")?.toIntOrNull() ?: 0
            ScreenshotScreen(rank = rank, repo = repo, onBack = { nav.popBackStack() })
        }
    }
}

@Suppress("UNCHECKED_CAST")
internal class GenericFactory(private val create: () -> ViewModel) : ViewModelProvider.Factory {
    override fun <T : ViewModel> create(modelClass: Class<T>, extras: CreationExtras): T =
        create() as T
}
