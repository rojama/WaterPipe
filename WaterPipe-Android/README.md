# WaterPipe Android 移植版

> 原作者：gchunyan（魅族 M8 版 v1.5）
> 移植版本：2.0.0-android | Kotlin + Jetpack Compose + MVVM

## 项目说明

本项目在独立目录 `WaterPipe-Android/` 中创建，**不会覆盖原 M8 (ARMV4I) 工程**（`/workspace/WaterPipe/`）。
所有改动推送至 git 分支 `waterpipe-android`。

## 相对原版的主要变化

### 1. 去除 DRM（授权/注册系统）
- 删除了 Common/DevInfo/md5 中 `IsValidRegKey / GetRegKey / GetMobleKey` 链路
- 删除了机器码、IMEI/SN/IMSI 获取、MD5 三轮重排、KMP key 文件校验
- 删除了 CopyRight 未注册弹窗流程；应用启动直接进入主界面
- 同时删除了 WaterPipeKeygen 算号器依赖（新分支不再包含该目录）

### 2. 修复的 Bug
| 原 Bug | 修复方式 |
|--------|---------|
| DevInfo SN 截断错位（写 `IMEI[17]` 而非 `SN[17]`） | 删除 DRM 相关代码，该路径不再存在 |
| Common::DirectoryExists 位运算优先级逻辑错 | 用 Kotlin File/Standard Ktx 替代 |
| GetMobleKey 中显式 `def.~DevInfo()` 双重析构 | 删除 DRM |
| FinalThread 中 ThreadData 多波泄漏 | 改为纯引擎 + 协程调度，无堆分配裸线程数据 |
| NewGame 首次 CloseHandle 未初始化句柄 | engine 用显式 `isFinalizing` 标志，无需句柄 |
| CreateThread 失败 ExitProcess(0) 杀进程 | 异常以 CancellationException 正常取消 |
| SettingWnd::SaveSetting 覆写 dwOrgVolume | "系统音量"不再直接修改全局状态，仅展示 |
| `IDR_WAVE_water_flow` 标签错配（实际为 klunk） | 重新在 SoundManager 中明确定义资源 |
| RankingList::DrawItem 中 `new RECT` 未释放 | Compose LazyColumn 无需手工 RECT |
| ErrAnimo new MemoryDC 未 delete | 用 Canvas composable 自动管理绘制 |
| SaveBmp 中 `delete lpBitmapBits` | Bitmap.compress + auto-close stream |

### 3. 性能优化
| 项目 | 原实现 | Android 版 |
|------|--------|-----------|
| 动画 | `CreateThread + Sleep(100)` + AlphaBlend 到共享 hdc | 协程 `delay` + Compose Canvas 增量重绘；每波由 Ticker channel 通知 |
| 并发 | 波内 ≤20 线程，波间 WaitForSingleObject | 单协程顺序处理波次，取消即结束；UI 由 Compose 重组自然并发 |
| 持久化 | INI 读写（`ReadWriteIni`）+ BMP 截图 | DataStore `preferencesDataStore` + Bitmap.compress PNG（无损更高质量、更小体积） |
| 播放音效 | `PlaySound` 重复打开资源 | `SoundPool` 预加载 + 按 Sfx 枚举 |
| 布局 | 硬编码像素坐标 390/90/395/430 等 | `BoxWithConstraints` + `weight/fill` 自适应各种横屏分辨率 |

### 4. 完善的功能
- 横屏自适应布局（`screenOrientation=sensorLandscape`），适配多种平板/手机
- 完整设置页：信息面板左右/队列方向上下/游戏音量/音效开关/返回键动作/说明/关于
- 排行榜：前 20 名带 PNG 终局截图，点击条目查看大图
- Material 3 主题（明/暗）
- 进榜对话框：姓名编辑（最多 7 字），自动记住上次姓名
- 跳过 / 开始注水按钮，游戏菜单（新游戏 / 开始注水 / 退出应用）
- 溢出飞溅动画 + 全程无缺 +20 奖励提示
- 应用无启动广告、不申请网络权限、不收集任何隐私数据

## 构建方法

> 本仓库为源码交付，已提供标准 Gradle 工程配置。首次构建需 Android Studio/AGP 下载 SDK 与依赖。

**环境要求**：
- Android Studio Iguana (或更新版)，JDK 17
- Android SDK 34 compileSdk / minSdk 24 / targetSdk 34
- Gradle 8.7 + AGP 8.5.2 + Kotlin 2.0

**构建步骤**：
```bash
# 1) 设置 ANDROID_SDK_ROOT
export ANDROID_SDK_ROOT=$HOME/Android/Sdk   # 或你的 SDK 路径

# 2) 进入 WaterPipe-Android 子项目
cd WaterPipe-Android

# 3) 首次下载依赖 + 编译
./gradlew :app:assembleRelease --no-daemon

# 产物路径
#   WaterPipe-Android/app/build/outputs/apk/release/app-release.apk
```

> 如环境中暂无 gradle wrapper 二进制（当前沙箱不保证可下载），可在有网络的环境
> 中执行一次 Android Studio "Sync with Gradle" 自动拉取 wrapper jar。

## 代码地图（Android 部分）

```
WaterPipe-Android/
 ├── settings.gradle.kts
 ├── build.gradle.kts
 ├── gradle.properties
 ├── gradle/wrapper/gradle-wrapper.properties
 ├── app/
 │   ├── build.gradle.kts
 │   ├── proguard-rules.pro
 │   └── src/main/
 │       ├── AndroidManifest.xml
 │       ├── java/com/gchunyan/waterpipe/
 │       │   ├── game/
 │       │   │   ├── PipeTypes.kt         (14 种 tag + 坐标/邻居)
 │       │   │   ├── BoxState.kt          (in_lab 改为 Set 去重)
 │       │   │   ├── FlowModels.kt
 │       │   │   └── WaterPipeEngine.kt   (纯 Kotlin 引擎 无 Android 依赖)
 │       │   ├── data/
 │       │   │   ├── Models.kt            (RankingEntry / AppSettings / HomeAction)
 │       │   │   └── SettingsRepository.kt (DataStore + 截图 PNG)
 │       │   ├── util/
 │       │   │   ├── SoundManager.kt      (SoundPool，修正原 water_flow 标签)
 │       │   │   └── BitmapUtil.kt
 │       │   └── ui/
 │       │       ├── Theme.kt
 │       │       ├── MainActivity.kt      (Navigation + GameViewModel)
 │       │       ├── GameScreen.kt        (主游戏：网格、预览面板、注水协程)
 │       │       ├── SettingsScreen.kt
 │       │       └── OtherScreens.kt      (排名榜/说明/关于/姓名/截图)
 │       └── res/
 │           ├── drawable/                (启动前景背景、矢量)
 │           ├── drawable-nodpi/          (从原工程复制的管道/背景 PNG 与 BMP)
 │           ├── layout/                  (Compose 不需)
 │           ├── mipmap-anydpi-v26/ic_launcher*.xml
 │           ├── raw/                     (8 个音效 wav)
 │           ├── values/strings.xml colors.xml themes.xml
 │           └── xml/network_security_config.xml
```

## 与原项目的目录关系

- 原 WinCE 工程：`/workspace/WaterPipe/` **保持不变**
- 新 Android 工程：`/workspace/WaterPipe-Android/`
- 新 git 分支：`waterpipe-android`（基于当前 master/agent 分支创建）

## 许可证

本仓库的 Android 移植部分遵循与原作者相同的非商业性使用约定；游戏内容版权归原作者 gchunyan 所有。
