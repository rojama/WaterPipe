# WaterPipe 水管工 — 项目 Code Wiki

> 版本：1.5 | 作者：gchunyan | 平台：魅族 M8 (Windows CE / WinCE, ARMV4I, MZFC SDK) | 构建工具：Visual Studio 2008 (vc80)

---

## 目录

1. [项目概述](#1-项目概述)
2. [目录结构](#2-目录结构)
3. [项目架构总览](#3-项目架构总览)
4. [核心模块与文件职责](#4-核心模块与文件职责)
5. [关键类说明](#5-关键类说明)
6. [数据结构与全局变量](#6-数据结构与全局变量)
7. [管道类型编码规则](#7-管道类型编码规则)
8. [核心流程详解](#8-核心流程详解)
9. [计分规则](#9-计分规则)
10. [授权系统 (DRM)](#10-授权系统-drm)
11. [资源系统](#11-资源系统)
12. [持久化 (INI + BMP)](#12-持久化--ini--bmp)
13. [线程与并发模型](#13-线程与并发模型)
14. [构建与运行](#14-构建与运行)
15. [已知问题与代码异味](#15-已知问题与代码异味)
16. [附录：常量速查](#16-附录常量速查)

---

## 1. 项目概述

WaterPipe（水管工）是第一款为魅族 M8 开发的收费益智类休闲游戏。玩家从 45 块随机水管队列中取放管道到 5×8=40 格网格上，连接从第 1 行第 3 格顶部出水口开始的水路，尽量不留缺口。放完所有管道后启动注水，水流经动画逐波推进，到达缺口时飞溅停止并计分；全程无缺口额外奖励 +20 分。支持本地排行榜（20 名，带 BMP 截图）、可热替换皮肤、音效开关、音量调节、M 键动作等设置。采用一机一码弱 DRM 做正版授权。

---

## 2. 目录结构

```
/workspace
├── WaterPipe/                         # 游戏主工程
│   ├── M8SDK (ARMV4I)/                # 编译输出 (obj + exe)
│   │   ├── Debug/                     #   Debug 版 WaterPipe.exe
│   │   └── Release/                   #   Release 版 WaterPipe.exe
│   ├── Resources/                     # 美术/音效资源源工程
│   │   ├── graphics/                  #   编译时使用的 PNG/BMP
│   │   ├── sounds/                    #   WAV + MIDI (MIDI 未被代码调用)
│   │   ├── Music/puzzle.mid           #   备用 MIDI
│   │   ├── Picture/                   #   参考截图 JPG
│   │   └── ORG_DATA/                  #   美术源 PSD / 分帧 PNG
│   │       ├── ACR/{正常水,荧光水}/    #     AllWater.png 分帧源
│   │       ├── PIPE/{大尺寸,正常,阴影}/ #    管道原始图
│   │       ├── 封面/ 排名榜/           #     BMP 背景
│   │       └── *.psd                  #     PhotoShop 工程
│   ├── WaterPipe.sln / .vcproj        # VS2008 解决方案/工程
│   ├── WaterPipe.rc / resource.h      # Win32 资源脚本
│   ├── main.h                         # ★ 全局头文件（结构体/宏/常量）
│   ├── WaterPipeMain.h                #   (孤立文件，与 main.h 重复定义，未被 #include)
│   ├── WaterPipeMain.cpp              # ★ 核心：游戏逻辑、动画、全部窗口
│   ├── Common.cpp/.h                  # 工具函数 + 注册校验
│   ├── DevInfo.cpp/.h                 # TAPI 取 IMEI/SN/IMSI
│   ├── md5.cpp/.h                     # MD5 实现
│   ├── Cover.cpp/.h                   # 启动封面窗口
│   ├── About.cpp/.h                   # 关于页
│   ├── Readme.cpp/.h                  # 游戏说明页
│   ├── CopyRight.cpp/.h               # 未注册/版权页
│   └── Ranking.cpp/.h                 # 排行榜 4 子窗口
├── WaterPipeKeygen/                   # 配套注册机
│   ├── keygen.bat                     #   批处理：java -jar keygen.jar
│   ├── keygen.jar                     #   Java 版算号器
│   ├── in.txt                         #   示例机器码输入
│   ├── key                            #   示例 key 文件（60 字符）
│   └── err.txt                        #   空文件
└── README.md
```

---

## 3. 项目架构总览

典型 MZFC（魅族 UI 框架）**单文档 + 多模态窗口**架构：

```
CMzApp (MainApp)
 ├── Init()
 │    ├── 显示 Cover 封面（非模态 Show，淡入）
 │    ├── 注册校验 Common::IsValidRegKey()
 │    ├─[失败]─→ 弹 CopyRight.DoModal() 显示机器码 → 退出
 │    └─[成功]─→ 载入 AllWater 动画图 → 读 app.ini → 创建 MainWnd
 │
 └── MainWnd (主游戏窗口，CMzWndEx)
      ├── 背景 / 网格 / 预览列表 / 跳过按钮 / 工具栏
      ├── 工具栏(0)="游戏" → CPopupMenu：退出/返回/流水/新游戏
      ├── 工具栏(1)="排名榜" → Ranking.DoModal
      │    └─ Ranking 内点击某项 → RankingBmp (查看终局截图)
      ├── 工具栏(2)="设置" → SettingWnd.DoModal
      │    ├── 面板位置/队列方向/系统音量/游戏音量/音效开关/M键动作
      │    ├── 调试信息(仅 V_DEBUG 编译时显示)
      │    ├── "游戏说明" → Readme.DoModal
      │    └── "关于游戏" → About.DoModal
      ├── 放管 → PutImage(h,v,skip)
      ├── 跳过 → PutImage(-1,-1,true)
      └── 全部放完 / 点"流水" → Final() → CreateThread(FinalThread)
           ├── 波次循环 while(!isErr && !now_chack_box.empty())
           │    ├── 本波 N 方块各起 FullAnimoThread (N<=20)
           │    ├── WaitForSingleObject 等本波全部完成
           │    └── GetNext() 推导下一波方块 (若溢出→isErr)
           ├── [isErr] ErrAnimo() 水滴飞溅
           ├── [!isErr] Score += 20
           ├── 终局画面 BitBlt 到全局 mdc (防止 PaintWin 重绘丢失)
           └── [进榜] RankingNameEdit → SaveList(INI+BMP) → Ranking.DoModal
```

窗口转场统一用：
- 显示：`MZ_ANIMTYPE_FADE` (淡入) 或 `MZ_ANIMTYPE_SCROLL_RIGHT_TO_LEFT_PUSH`
- 隐藏：`MZ_ANIMTYPE_FADE` (淡出) 或 `MZ_ANIMTYPE_SCROLL_LEFT_TO_RIGHT_PUSH`

---

## 4. 核心模块与文件职责

| 文件 | 行数(约) | 核心职责 |
|------|---------|---------|
| [main.h](file:///workspace/WaterPipe/main.h) | 135 | 全局头：控件 ID 宏、尺寸常量、**数据结构声明**、前向声明 |
| [WaterPipeMain.cpp](file:///workspace/WaterPipe/WaterPipeMain.cpp) | 2893 | ★ 全部游戏逻辑：动画算法、窗口类 (`PreviewList`/`Scorbar`/`SettingWnd`/`MainWnd`/`MainApp`)、线程函数 (`FinalThread`/`FullAnimoThread`)、放管/新游戏/计分/设置读写 |
| [Common.cpp](file:///workspace/WaterPipe/Common.cpp) | 600+ | 工具：程序目录、文件存在性、目录存在性、BMP 保存、GUID、日期时间、**注册校验三函数** (`IsValidRegKey`/`GetRegKey`/`GetMobleKey`)、KMP 子串匹配 |
| [DevInfo.cpp](file:///workspace/WaterPipe/DevInfo.cpp) | 280+ | 封装 TAPI `lineGetGeneralInfo` → 解析得到 **IMEI / SN / IMSI**（`Siminfo` 结构体） |
| [md5.cpp](file:///workspace/WaterPipe/md5.cpp) | — | 标准 MD5 类 `CMd5`：`TargetStr(const char*)` 输入、`GetDigestKey()` 返回 32 位十六进制 LPCSTR |
| [Cover.cpp](file:///workspace/WaterPipe/Cover.cpp) | — | 封面窗口：单张背景图按钮，仅 `Show()` 显示，靠 MainWnd 创建覆盖 |
| [About.cpp](file:///workspace/WaterPipe/About.cpp) | — | 关于页：滚动显示程序名/版本 1.5/作者/授权声明 |
| [Readme.cpp](file:///workspace/WaterPipe/Readme.cpp) | — | 玩法说明页：含完整计分规则文本 |
| [CopyRight.cpp](file:///workspace/WaterPipe/CopyRight.cpp) | — | 未注册页：调用 `GetMobleKey` 显示机器码 + 注册说明文字 |
| [Ranking.cpp](file:///workspace/WaterPipe/Ranking.cpp) | 400+ | 4 个窗口：`RankingList`(自绘列表)、`Ranking`(榜单)、`RankingNameEdit`(姓名输入)、`RankingBmp`(查看截图) |
| [WaterPipe.rc](file:///workspace/WaterPipe/WaterPipe.rc) + [resource.h](file:///workspace/WaterPipe/resource.h) | — | PNG/PIPE1-14、AllWater、水滴 BMP、YuLang/WangGe/Ranking_head、5 个 WAV 资源 |

> **注**：[WaterPipeMain.h](file:///workspace/WaterPipe/WaterPipeMain.h) 是**孤立头文件**，全项目无任何 `#include "WaterPipeMain.h"`，内容与 `main.h` 大量重复并额外定义了死代码 `SoundsData`/`IPlayerCore_Play`。若被引用会触发结构体重定义编译错误。

---

## 5. 关键类说明

### 5.1 MainApp — 应用入口

定义位置：[WaterPipeMain.cpp#L2549](file:///workspace/WaterPipe/WaterPipeMain.cpp#L2549)

继承自 `CMzApp`。

| 成员 | 类型 | 作用 |
|------|------|------|
| `m_MainWnd` | MainWnd | 主游戏窗口实例 |
| `Init()` | 方法 | **启动总流程**：CoInitialize → 禁止锁屏 → 显示 Cover → 校验注册 → [否]弹 CopyRight / [是]设置音量/AlphaBlend/载入 AllWater/创建主窗口 |

全局实例：`MainApp theApp;` ([L2669](file:///workspace/WaterPipe/WaterPipeMain.cpp#L2669))

---

### 5.2 MainWnd — 主游戏窗口

定义位置：[WaterPipeMain.cpp#L1906](file:///workspace/WaterPipe/WaterPipeMain.cpp#L1906)

继承自 `CMzWndEx`，`MZ_DECLARE_DYNAMIC`/`MZ_IMPLEMENT_DYNAMIC` 宏支持 RTTI。

#### 成员控件

| 成员 | 类型 | ID 宏 | 作用 |
|------|------|-------|------|
| `m_Toolbar` | UiToolbar_Text | PIPE_TOOLBAR(120) | 底部三按钮工具栏：0=游戏/1=排名榜/2=设置 |
| `but_skip` | UiButton | PIPE_MAIN_BTN_SKIP(101) | "跳过"(游戏中) 或 "开始"(结束后) 橙色大按钮 |
| `p_List` | PreviewList | PIPE_MAIN_PRELIST(140) | 待放管道预览队列（自绘 5 格可见） |
| `but_bg` | UiButton_Image | — | 主背景 (Main_bg.bmp) |
| `but_WangGe` | UiButton_Image | — | 网格底图 (WangGe.png) |
| `but_YuLang` | UiButton_Image | — | 语浪装饰图 (YuLang.png) |
| `but_1_1 ~ but_8_5` | UiButton_Image ×40 | PIPE_MAIN_BTN_1_1..5_5(211..255+311..355) | 网格按钮，按下标顺序 push 到 `but_all` |
| `static_score` / `static_level` | UiStatic ×2 | — | 分数/剩余数标签 |

#### 成员方法

| 方法 | 位置 | 作用 |
|------|------|------|
| `OnInitDialog()` | L1980 | 初始化全部：加载 15 张管道 PNG 到 `pipe_img_all`，获取全局 `hdc`/`hWnd`，初始化水滴掩码，读设置，按 `isPreLeft` 左/右布局背景/网格/信息/预览/跳过/40 格按钮，最后 **`NewGame()`** |
| `PaintWin(hdc, prcUpdate)` | L2190 | 基础 PaintWin 后，若 `isInRepain` 则从 `mdc` BitBlt 回终局画面（避免重绘丢失动画结果） |
| `OnMzCommand(wParam, lParam)` | L2211 | 工具栏弹出菜单（新游戏/流水/退出）、排名榜 `Ranking.DoModal`、设置 `SettingWnd.DoModal` |
| `OnShellHomeKey()` | ~L2203 | M 键按下 → 恢复音量 `SetVolume(dwOrgVolume)` → 返回 `mAction`（关闭/最小化/无效） |
| `PutImage(h, v, skip)` | L2321 | 核心放置函数：从 `p_List` 取当前块 → 写到目标格 `box_static[but_no].tag`；若与已有块互补则合并为 `LURD\`/`LURD/` 立交桥；播放音效；刷新剩余/分数；剩余为 0 时 `Final()` |
| `Final()` | L2421 | `CreateThread(FinalThread)` 启动注水流程线程 |
| `NewGame()` | L2434 | 重置：清状态/清动画/CloseHandle(hFinalThread)；`srand(年+月+日+时+分+秒)`；随机 45 块 `rand()%12`（Pipe1..Pipe12）进 `p_List`；40 格清空；`but_no==2` 设 `in_lab="U"` 作为水源；刷新标签 |

---

### 5.3 SettingWnd — 设置窗口

定义位置：[WaterPipeMain.cpp#L1441](file:///workspace/WaterPipe/WaterPipeMain.cpp#L1441)

| 成员 (分组) | 控件 | ID 宏 |
|------------|------|-------|
| 界面组 | m_Caption1 / m_BtnSetting_PreWhere(面板左右) / m_BtnSetting_PipeDire(队列上下) | PIPE_SETTING_PIPEWHERE(158) / PIPE_SETTING_PIPEDIRE(153) |
| 声音组 | m_Caption2 / m_Static_SysVolume+m_StatusBar_SysVolume / m_Static_Volume+m_StatusBar_Volume / m_BtnSetting_SoundLable+m_BtnSetting_SoundSwitch | PIPE_SETTING_SYSVOLUME(159) / PIPE_SETTING_VOLUME(152) / PIPE_SETTING_SOUND(150) |
| 调试(V_DEBUG) | m_BtnSetting_DebugLable+m_BtnSetting_DebugSwitch | PIPE_SETTING_DEBUG(151) |
| 其它组 | m_Caption3 / m_BtnSetting_MKeyAction / m_BtnSetting_readme / m_BtnSetting_about | PIPE_SETTING_MKEYACT(160) / PIPE_SETTING_README(154) / PIPE_SETTING_ABOUT(155) |

#### 静态方法

| 方法 | 位置 | 作用 |
|------|------|------|
| `ReadSetting()` | L1468 | 若 `app.ini` 不存在则创建；读 `[WaterPipe]`：Volume/isSoundsOn/isdebug/isPreLeft/isPreListUp/MKeyAction；读 `[Ranking]`：name_N+score_N → `ranking.addRanking` → 赋 `ranking_last_name` |

#### 实例方法

| 方法 | 位置 | 作用 |
|------|------|------|
| `OnInitDialog()` | L1543 | 创建滚动容器 m_ScrollWin，依次按 y 累加布局三组所有控件，底部工具栏(取消/保存) |
| `OnMzCommand()` | L1715 | 每项设置点击即时切换显示；工具栏：取消→EndModal(ID_CANCEL)；保存→`SaveSetting()`→EndModal(ID_OK) |
| `SaveSetting()` | L1818 | 取滑块/开关值 → 写 `[WaterPipe]` 的 6 项到 INI；系统音量滑块值**覆盖**全局 `dwOrgVolume` 并立即 SetVolume |

---

### 5.4 Scorbar — 自定义滑块

定义位置：[WaterPipeMain.cpp#L1427](file:///workspace/WaterPipe/WaterPipeMain.cpp#L1427)

继承 `CUiStatusBar`。新增成员 `UiButtonEx* sta`；重写 `OnMouseMove`：拖动时将百分比 `GetPadpos()*100/0xFFFF` 写到 `sta->SetText2` 刷新显示。

---

### 5.5 PreviewList — 管道队列预览列表

定义位置：[WaterPipeMain.cpp#L1878](file:///workspace/WaterPipe/WaterPipeMain.cpp#L1878)

继承 `UiList`。重写 `DrawItem`：选中项绘高亮，`pItem->Data!=NULL` 时把 `ImagingHelper*` 绘到项矩形中。

---

### 5.6 Ranking — 排行榜主窗口

定义位置：[Ranking.cpp#L48](file:///workspace/WaterPipe/Ranking.cpp)

继承 `CMzWndEx`。

| 成员 | 类型 | ID 宏 |
|------|------|-------|
| `m_List` | RankingList | PIPE_MAIN_RANKING_LIST(141) |
| `m_Toolbar` | UiToolbar_Text | PIPE_RANKING_LIST_TOOLBAR(142) |
| `but_bg` / `but_head` | UiButton_Image | Ranking_bg.bmp + Ranking_head.png |
| `highlight` | int | (输入)要高亮的名次，进榜时传入 rankno |
| `showNew` | bool | (输入)是否显示"新游戏"按钮 |

#### 方法

| 方法 | 位置 | 作用 |
|------|------|------|
| `getRankingNo(score)` | L56 | 遍历列表找到比 score 小的第一项名次 i+1；不满 20 名则尾部；否则 -1 |
| `addRanking(name, score)` | L77 | 尾部加一项 |
| `insertRanking(no, name, score)` | L86 | 插入第 no 名，超 20 时 RemoveItem(20) |
| `OnInitDialog()` | L103 | 加载 bg+head，列表项高 55 像素，高亮 highlight-1 项，工具栏(返回/新游戏) |
| `MzDefWndProc()` | L158 | 列表项 LBUTTONUP → 创建 `RankingBmp.DoModal` 看终局截图 |
| `SaveList(from, RankingName, SettingFileName)` | L218 | 从第 from 名起写 `[Ranking]` name_N/score_N 到 INI；同名次 bmp **从尾往前 MoveFile** 重命名顺延 rank_N.bmp |
| `OnMzCommand()` | L242 | 工具栏返回→ID_CANCEL；新游戏→ID_OK |

---

### 5.7 RankingList — 自绘列表项

定义位置：[Ranking.cpp#L9](file:///workspace/WaterPipe/Ranking.cpp)

继承 `UiList`。`DrawItem` 高亮项/选中项后绘文字 `第X名：Y分　姓名`，末尾绘箭头图标。注意 `new RECT` 未释放，滚动时泄漏内存。

---

### 5.8 RankingNameEdit — 进榜姓名输入

定义位置：[Ranking.cpp#L273](file:///workspace/WaterPipe/Ranking.cpp)

| 成员 | 作用 |
|------|------|
| `rankno` / `score` | (输入)名次+分数，显示 "恭喜你获得了第X名，得分Y分。" |
| `name` | (输入/输出)默认 ranking_last_name，确定后回写 |
| `lineedit` | 单行编辑框，max 7 字符，IM_SIP_MODE_GEL_PY 拼音输入法 |
| `edit_Toolbar` | 工具栏：0="不记录"(ID_CANCEL)，2="记录"(ID_OK) |

---

### 5.9 RankingBmp — 查看终局截图

定义位置：[Ranking.cpp#L369](file:///workspace/WaterPipe/Ranking.cpp)

`rankno`(输入) → 加载 `ranking\rank_N.bmp` → `but_bg` 全屏显示。无图则显示 "记录图像不存在"。

---

### 5.10 Common 工具类

定义位置：[Common.cpp](file:///workspace/WaterPipe/Common.cpp)

全静态方法：

| 方法 | 位置 | 作用 |
|------|------|------|
| `GetProgramDir()` | L14 | `GetModuleFileName` + 截去 exe 名 → 安装目录 |
| `SaveBmp(mdc, filename)` | L31 | 16-bit CreateDIBSection → BitBlt → 写 BMP 文件头+InfoHeader+像素；注意 `delete lpBitmapBits` 是错误写法 |
| `OpenFile(filename, &text)` | — | `_wfopen` 整个文件读入 `char*` |
| `FileExists(path)` / `DirectoryExists(path)` | — | 分别用 GetFileAttributes 判断 |
| `GetGuidString()` | L529 | `CoCreateGuid` + `StringFromCLSID`（`CoTaskMemFree` 被注释） |
| `Date()` / `Time()` / `Now()` | ~L566 | 格式化日期时间字符串（`Now()` 被注释掉返回空串） |
| `kmp_init`/`kmp_find`/`isFind` | L180 | 标准 KMP 算法，用于 key 文件内子串查找 |
| `IsValidRegKey()` | L225 | **注册入口**：读安装目录 key 文件 → GetMobleKey → GetRegKey → isFind 命中则通过 |
| `GetRegKey(szIn, szKey)` | L244 | **三轮 MD5 重排**推导 30 字符注册码 |
| `GetMobleKey(szDes)` | L362 | **IMEI+SN MD5 交叉取字符**拼 19 字符机器码（含 3 个 `-`） |

---

### 5.11 DevInfo 设备信息类

定义位置：[DevInfo.cpp](file:///workspace/WaterPipe/DevInfo.cpp)

| 方法 | 作用 |
|------|------|
| `Init()` | `lineInitializeEx` → `lineOpen` |
| `GetGeneralInfo()` | `lineGetGeneralInfo` → 解析 LINEGENERALINFO 的 Manufacturer/Model/Revision/SerialNumber/SubscriberNumber 字段 → 填 `Siminfo{IMEI, SN, IMSI}`；SN>17 时截断错位写到 `IMEI[17]` (复制粘贴 Bug) |
| `GetTAPIErrorMsg()` | `FormatMessage(TAPIERROR_FORMATMESSAGE(...))` |

---

### 5.12 Cover / About / Readme / CopyRight 辅助窗口

| 类 | 继承 | 内容 |
|----|------|------|
| Cover | CMzWndEx | 单 `but_bg` 图片，`SetBgColor(NULL)` + `AnimateWindow(FADE)` 淡入 |
| About | CMzWndEx | `UiScrollWin` 滚动显示作者+版本+授权的多行文字 |
| Readme | CMzWndEx | 游戏说明文本 + 完整计分规则 |
| CopyRight | CMzWndEx | 机器码显示（调用 `Common::GetMobleKey`） + 注册说明 |

---

## 6. 数据结构与全局变量

### 6.1 核心结构体

全部定义在 [main.h#L102-L132](file:///workspace/WaterPipe/main.h#L102)。

```
BoxStatic (每格状态，索引=方块号 0..39)
  tag        CMzStringW  管道类型编码，如 "LURD" / "LU" / ""
  in_lab     CMzStringW  当前"水从哪些口注入"，如 "U" / "LU"
  full_lab   CMzStringW  已充填的口（动画中使用，很少读）

BoxAnimo (声明但实际很少被代码使用)
  box        int         方块号
  from_to    vector<CMzStringW>  动画方向分解

ErrBoxAnimo
  box        int         溢出方块号
  err_lab    CMzStringW  溢出部位："L"/"R"/"U"/"D"

ThreadData (传给 FullAnimoThread 的参数)
  box_no     int
  box        UiButton_Image*
  box_static PBoxStatic

ErrThreadData (声明但未使用)
  box        UiButton_Image*
  err_lab    CMzStringW
```

### 6.2 全局变量 (WaterPipeMain.cpp 顶部)

| 变量 | 类型 | 位置 | 作用 |
|------|------|------|------|
| `hdc` | HDC | L11 | 全局窗口 DC（全部动画线程直接在此绘） |
| `hWnd` | HWND | L12 | 主窗口句柄 |
| `rcWork` | RECT | L13 | `MzGetWorkArea()` 工作区（720×480） |
| `AppPath` | CMzString | L21 | `GetProgramDir()` 安装目录 |
| `SettingFileName` | CMzString | L22 | 实际= `AppPath+"app.ini"` |
| `hFinalThread` / `dwFinalThreadId` | HANDLE/DWORD | L23 | FinalThread 句柄/ID |
| `dwVolume` / `dwOrgVolume` | DWORD | L27 | 游戏音量/启动时原系统音量（后被 SaveSetting 覆写） |
| `isInGame` / `isInRepain` | bool | L29 | 是否在游戏中 / 终局画面是否需从 mdc 恢复 |
| `isSoundsOn` / `isdebug` | bool | L31/35 | 音效/调试开关 |
| `isPreLeft` / `isPreListUp` | bool | L32/36 | 信息面板左右 / 队列上下（`org_` 为游戏启动时快照） |
| `mAction` | int | L34 | M 键返回值：`SHK_RET_APPEXIT_SHELLTOP`(关闭) / `SHELLNOTOP`(最小化) / `APPNOEXIT_SHELLNOTOP`(无效) |
| `now_item_no` | int | L38 | 当前待放项在队列中的偏移（PutImage 中递增） |
| `pipe_img_all` | vector<ImagingHelper*> | L39 | 15 张管道 PNG，at(0..14)=Pipe0..Pipe14 |
| `but_all` | vector<UiButton_Image> | L40 | 40 格网格按钮，at(0..39) |
| `box_static` | vector<BoxStatic> | L42 | 40 格状态，at(0..39) |
| `now_chack_box` | vector<int> | L41 | 当前波次要处理的方块号列表（BFS 队列） |
| `box_err_animo` | vector<ErrBoxAnimo> | L45 | 溢出方块列表（ErrAnimo 使用） |
| `isErr` | bool | L46 | 是否发生溢出（终止主循环） |
| `Score` | int | L47 | 当前游戏分数 |
| `ranking` | Ranking | L49 | 全局排行榜对象，读入后常驻 |
| `mdc` | MemoryDC* | L51 | 终局画面缓存（全局 new，从未释放） |
| `bf` | BLENDFUNCTION | L55 | 固定：AC_SRC_OVER / 255 / AC_SRC_ALPHA（AlphaBlend 参数） |
| `img_AllWater` | ImagingHelper* | L56 | AllWater.png 精灵图（水流动画源） |
| `pimg_droplets` | ImagingHelper* | L15 | droplets24bit.bmp（水滴飞溅源） |

---

## 7. 管道类型编码规则

方块的 `tag` 字符串由 L(左)、R(右)、U(上)、D(下) 四个字母的**子集并按 L<R<U<D** 顺序拼接表示。共 14 种类型：

| tag | 形状 | PNG ID | 说明 |
|-----|------|--------|------|
| `LR` | 横直线 | Pipe1 | |
| `UD` | 竖直线 | Pipe2 | |
| `LUR` | T 型（左+上+右开口） | Pipe3 | 丁字 1 |
| `URD` | T 型（上+右+下开口） | Pipe4 | 丁字 2 |
| `LRD` | T 型（左+右+下开口） | Pipe5 | 丁字 3 |
| `LUD` | T 型（左+上+下开口） | Pipe6 | 丁字 4 |
| `LURD` | 十字 | Pipe7 | 四向贯通 |
| `LURDX` | 立交（十字过桥，两向独立） | Pipe8 | 横向/纵向彼此独立通过 |
| `LU` | 左上弧（L 型） | Pipe9 | 四分之一圆 |
| `RU` | 右上弧 | Pipe10 | |
| `RD` | 右下弧 | Pipe11 | |
| `LD` | 左下弧 | Pipe12 | |
| `LURD\` | 双弧立交（LU+RD 两对角） | Pipe13 | 放置时由互补块合并产生 |
| `LURD/` | 双弧立交（LD+RU 两对角） | Pipe14 | 放置时由互补块合并产生 |
| `""` | 空（无管道） | Pipe0 或 none | 未放置 / 已放置 none |

**随机生成**：`NewGame()` 中 `rand()%12`，即只从 Pipe1..Pipe12 中随机（不含 X、\、/ 三种立交）。

**立交桥产生**：`PutImage` 放置时若 `(旧tag+新tag)` 等于一组互补对，则升级：
- `LU+RD` → `LURD\` (Pipe13)
- `LD+RU` → `LURD/` (Pipe14)

---

## 8. 核心流程详解

### 8.1 启动与注册流程

```
MainApp::Init() [L2556]
 ├── CoInitializeEx(COINIT_MULTITHREADED)
 ├── SetPowerRequirement("BKL1:", D0)   // 禁止锁屏
 ├── Load Cover_bg.bmp → Cover.Show(FADE)
 ├── BOOL go = Common::IsValidRegKey()  // 读安装目录下 key 文件
 │     ├── OpenFile("key", &text)
 │     ├── GetMobleKey(szKey)          // 本机 IMEI+SN → 19 字符机器码
 │     ├── GetRegKey(szKey, szOutKey)  // 三轮 MD5+重排 → 30 字符注册码
 │     └── isFind(text, szOutKey)      // KMP 子串命中即通过
 │
 ├─ go == FALSE ──→ CopyRight.DoModal() 显示机器码 + 注册说明
 │
 └─ go == TRUE  ──→
       ├── waveOutGetVolume → dwOrgVolume  (保存原系统音量)
       ├── AlphaBlend 参数 bf 初始化
       ├── 加载 AllWater.png (IDB_PNG_AllWater)
       ├── SettingFileName = AppPath + "app.ini"
       └── m_MainWnd.Create + AnimateWindow(FADE) + Show
```

### 8.2 新游戏初始化

`MainWnd::NewGame()` [L2434](file:///workspace/WaterPipe/WaterPipeMain.cpp#L2434)

1. **重置标志**：清 isErr/isInRepain/Score，清 box_err_animo/now_chack_box/animo
2. **CloseHandle(hFinalThread)**（首次调用时 hFinalThread 未初始化）
3. **生成 45 管道队列**：
   - `srand(nYear+nMonth+nDay+nHour+nMinute+nSecond)`
   - 45 次 `rand()%12` → pipe_img_all.at(a+1) + tag 文本 → p_List.AddItem
   - 根据 isPreListUp 在队头或队尾补 5 个 none 占位
4. **清空 40 格**：SetImage_Normal(NULL)、tag/in_lab/full_lab=""
5. **水源**：`but_no==2`（第 1 行第 3 格，0 下标=2）→ `in_lab="U"`，加入 now_chack_box
6. **刷新标签**：剩余 45、分数 0

### 8.3 放置管道 (PutImage)

`MainWnd::PutImage(h, v, skip)` [L2321](file:///workspace/WaterPipe/WaterPipeMain.cpp#L2321)

| 参数 | 含义 |
|------|------|
| h, v | 目标格子行列（1-based；skip 模式下 =-1,-1） |
| skip | true=跳过当前块，不放置 |

流程：
1. 若 p_List 队列为空则 return
2. `now_item_no` 根据 isPreListUp 取下一个索引
3. 取 ListItem → pipe_img=Data, tag=Text
4. skip=false 时：
   - 计算 but_no = (v-1)*5 + (h-1)
   - 若 **旧 tag 与新 tag 互补** (如 `LU+RD`) 则升级为 `LURD\`/`LURD/` → PlaySound(klunk.wav)
   - 否则写 tag，SetImage_Normal → PlaySound(assembly_line_moving)
5. now_item_no++；刷新剩余数；
6. **若 now_item_no >= PIPE_MAIN_NUM_IMG(45)** → 全部放完 → `Final()`

### 8.4 注水总流程 (FinalThread)

`DWORD WINAPI FinalThread(LPVOID)` [L2673](file:///workspace/WaterPipe/WaterPipeMain.cpp#L2673)

```
0. 禁用全部按钮(工具栏/跳过/40 格)
1. Sleep(1000)；isInGame=false
2. ChackFirst() —— 对 now_chack_box=[水源 but_no==2] 校验首格
      若 in_lab 含某方向但 tag 不含该方向 → 记为溢出 + isErr=true
3. while (!isErr && !now_chack_box.empty())
   ├─ [波次 i] 本波方块数=now_chack_box.size()
   ├─ PlaySound(water_flow, LOOP|ASYNC)
   ├─ for 每个方块 now in now_chack_box
   │    ├─ AddScore(now)  先加分
   │    ├─ HeapAlloc(ThreadData) pData
   │    └─ CreateThread(FullAnimoThread, pData)  ≤20 个并发
   ├─ for 每个线程 WaitForSingleObject 串行等待完成
   ├─ 清空 temp_chack_box
   ├─ for 每个方块 now in now_chack_box
   │    └─ GetNext(now) ——
   │          对当前 tag×每个入口方向 查表推导出"流向相邻格方向"
   │            ├─ 目标格存在 → 下一波加入，累加其 in_lab
   │            └─ 目标格不存在(边界外) → 记溢出 ErrBoxAnimo，isErr=true
   ├─ 去重合并 temp_chack_box → now_chack_box → 进入下一波
4. Stop 水流声；恢复音量
5. if isErr → PlaySound(warning) → ErrAnimo() ×3 轮水滴飞溅
   else    → Score += 20 (全程无缺奖励)
6. 恢复按钮；跳过按钮→"开始"
7. 终局画面 BitBlt → 全局 mdc；isInRepain=true
8. int rankno = ranking.getRankingNo(Score)
   ├─ rankno != -1 (进榜)
   │    ├─ MemoryDC save_mdc 抓全屏
   │    ├─ RankingNameEdit.DoModal：编辑姓名
   │    ├─ [ID_OK] ranking.insertRanking → SaveList(INI+BMP 重命名顺移)
   │    ├─ SaveBmp(save_mdc, "ranking\rank_rankno.bmp")
   │    └─ Ranking.DoModal(高亮+新游戏按钮) → ID_OK 时 NewGame()
   └─ rankno == -1 (未进榜) → 结束
```

### 8.5 GetNext 方向推导 (关键函数)

`vector<int> GetNext(int box_no)` [L329](file:///workspace/WaterPipe/WaterPipeMain.cpp#L329)

针对 **tag × in_lab 组合**，对每个入口方向按"水流进入后从对应口流出"规则：
- `LR` 入 L → 出 R → 东侧邻格，其 in_lab 加 "L"（接受来自西侧的水）
- `LU` 入 L → 出 U → 北侧邻格，其 in_lab 加 "D"
- `LURD`（十字）多入口按每口分别推
- `LURDX`（过桥）横纵独立，处理后强制 `in_lab=""` 清空
- `LURD\`/`LURD/`（双弧立交）两对角独立，处理后清空 in_lab

用 `SetRuleForNext(&ret, box_no, 'W'|'E'|'N'|'S')` 统一：
```
'W'→流向西→西邻→in_lab="R"→边界溢出则"L"错
'E'→流向东→东邻→in_lab="L"→边界溢出则"R"错
'N'→流向北→北邻→in_lab="D"→边界溢出则"U"错
'S'→流向南→南邻→in_lab="U"→边界溢出则"D"错
```
边界判定 `GetBoxInDirection(box_no, direction)` 返回值：
- `>=0` ：目标方块号
- `==-2` ：越出边界 → 记溢出错误

### 8.6 FullAnimoThread 逐帧动画

每格一种动画，范围约 [WaterPipeMain.cpp#L762-L1425](file:///workspace/WaterPipe/WaterPipeMain.cpp#L762)。手写 `case "tag" + case "入口组合"` 对每种组合做 15 帧 AlphaBlend 绘制。

核心辅助函数：
- `SubLineAnimo(hdc, sdc, from_dir, to_dir, frame, x, y)` — 直线帧，从 AllWater.png 取行偏移；帧数过半算半格 (`HAFE_FRAME`=8)
- `SubArcAnimo(hdc, sdc, from_dir, to_dir, frame, x, y)` — 弧帧，按方向算 AllWater.png 中子图左上角；HDR 混合裁剪区 PIPE_MAIN_SIZE_ACR_DESC(20)/ACR_IMG(55)

每帧后 `Sleep(PIPE_ANIMO_FRAME_BETWEEN=100ms)`。结束前写 `box_static->full_lab`。

---

## 9. 计分规则

函数：`AddScore(box_no)` [WaterPipeMain.cpp#L84](file:///workspace/WaterPipe/WaterPipeMain.cpp#L84)

| 管道 (tag) | 条件 | 分值 |
|-----------|------|------|
| 弧线 LU/RU/RD/LD (2 字符且非 LR/UD) | 无条件 | 6 |
| 直线 LR/UD | 无条件 | 7 |
| 丁字 LUR/URD/LRD/LUD (3 字符) | 无条件 | 10 |
| 十字 LURD | 无条件 | 13 |
| 立交 LURDX | 横向+纵向**均贯通**(in_lab 同时含 LR + UD) | 16 |
| 立交 LURDX | 仅贯通单向 | 8 |
| 双弧立交 LURD\ | 两对角均贯通(UR+LD) | 12 |
| 双弧立交 LURD\ | 仅一对角贯通 | 6 |
| 双弧立交 LURD/ | 两对角均贯通(UL+DR) | 12 |
| 双弧立交 LURD/ | 仅一对角贯通 | 6 |
| **全程无缺奖励** | FinalThread 末尾 `!isErr` | **+20** |

读设置时从 ini `[Ranking]` 载入前局排行，新分数超过榜尾即进榜（最多 20 名）。

---

## 10. 授权系统 (DRM)

### 10.1 链路图

```
 用户手机
   └── TAPI lineGetGeneralInfo
         ├── SerialNumber → IMEI[]
         └── Model[]       → SN[]
                    │
                    ▼
         GetMobleKey()  [Common.cpp#L362]
         MD5(IMEI)[15],  MD5(SN)[27,30,3,25,23,22,31,5]
         交叉取 16 字符 + 3 个 '-' → 19 字符机器码，形如
         "2b33-3a0e-4faf-f555"
                    │
                    ▼
         GetRegKey(mobleKey)  [Common.cpp#L244]
           ├─ 轮 1: MD5(in) → 按下标 [15,18,17,27,4,14,11,28,7,31,1,6,
           │   16,22,2,13,29,8,5,10,3,9,12,19,21,23,24,25,30,26] 取 30 字符
           ├─ 轮 2: MD5(轮1 结果) → 另取下标 重排 30 字符
           └─ 轮 3: MD5(轮2 结果) → 再取下标重排 30 字符注册码 szKey
                    │
                    ▼
         IsValidRegKey()  [Common.cpp#L225]
         读安装目录下 "key" 文件 → KMP(isFind) 搜索 szKey
           命中 → 正版授权；未命中 → 弹 CopyRight 机器码页
```

### 10.2 Keygen

目录：[WaterPipeKeygen/](file:///workspace/WaterPipeKeygen)

| 文件 | 内容 |
|------|------|
| `keygen.bat` | `java -jar keygen.jar` |
| `keygen.jar` | Java 版算号器（与上述 GetRegKey 同算法） |
| `in.txt` | 示例输入：`2b33-3a0e-4faf-f555` |
| `key` | 60 字符（= 2 个 30 字符注册码拼接，用于 KMP 任取一段命中） |
| `err.txt` | 空（疑似错误输出） |

**安全强度**：客户端纯校验，算法公开（keygen 同目录）。去掉 `IsValidRegKey` 校验或补丁恒真即可破解。

---

## 11. 资源系统

### 11.1 管道 PNG (RCDATA 编译进 exe)

[WaterPipe.rc](file:///workspace/WaterPipe/WaterPipe.rc)：
```
IDB_PNG_PIPE0..14  → RCDATA "Resources/graphics/PipeX.png"
IDB_PNG_PIPE_NONE  → RCDATA "Resources/graphics/none.png"
IDB_PNG_AllWater   → RCDATA "Resources/graphics/AllWater.png"
IDB_PNG_WangGe     → RCDATA "Resources/graphics/WangGe.png"
IDB_PNG_YuLang     → RCDATA "Resources/graphics/YuLang.png"
IDB_PNG_Ranking_head → RCDATA "Resources/graphics/Ranking_head.png"
IDB_BITMAP_droplets  → RCDATA "Resources/graphics/droplets24bit.bmp"
```
加载统一：`ImagingHelper::GetImageObject(MzGetInstanceHandle(), ID, true)`。

### 11.2 背景 BMP 皮肤 (文件系统加载，可热替换)

| 场景 | 加载位置 | 文件 |
|------|---------|------|
| 封面 | MainApp::Init L2563 | AppPath + "Cover_bg.bmp" |
| 主背景 | MainWnd::OnInitDialog L2058 | AppPath + "Main_bg.bmp" |
| 排行榜背景 | Ranking::OnInitDialog L114 | RankAppPath + "Ranking_bg.bmp" |

README 声明："增加自由更换皮肤，将游戏目录下的三个 bmp 进行替换即可。" —— 即直接覆盖安装目录下这三个同名 bmp 文件，无需重新编译。

### 11.3 水流皮肤

Resources/ORG_DATA/ACR/ 下两套美术源：
- `正常水/Acr_{DR,LD,RU,UL}/Full_Acr_01..15.png`（4 方向 × 15 帧）
- `荧光水/Acr_{DR,LD,RU,UL}/Full_Acr_01..15.png`

用 `CssBgImageMergeTool.exe` + `ImageManipulation.dll` 合成 `AllWater.png`。**运行时代码只编译了一份**（正常水版），"荧光水"需替换资源后重新编译方可切换。

### 11.4 声音

- 实际播放用 **Win32 `PlaySound(MAKEINTRESOURCE(IDR_WAVE_*), SND_RESOURCE|SND_ASYNC)`**：
  | ID | 实际文件 | 场景 |
  |----|---------|------|
  | IDR_WAVE1 | Button.wav (assembly_line_moving) | 放管 |
  | IDR_WAVE2 | boink.wav (klunk) | 立交桥合并 |
  | IDR_WAVE3 | glass_breaking.wav | — |
  | IDR_WAVE4 | warning.wav | 溢出报警 (SND_LOOP) |
  | IDR_WAVE_water_flow | **klunk.wav** (注意 RC 标签错配，非 water_flow.wav) | 水流 (SND_LOOP) |

- 音量控制：`waveOutSetVolume(0, MAKELPARAM(dwVolume,dwVolume))`，游戏结束恢复 `dwOrgVolume`
- **MIDI 背景乐未启用**：Resources/sounds/ 下 12 个 MID，以及 IPlayerCore.h/SoundsData 结构体均为死代码，全项目无 `IPlayerCore_Play::Open/Play` 调用。

---

## 12. 持久化 (INI + BMP)

文件：`AppPath + "app.ini"`，使用 MZFC `ReadWriteIni` 接口。

### 12.1 [WaterPipe] 段

| Key | 类型 | 默认 | 含义 |
|-----|------|------|------|
| Volume | int | 0xFFFF | 游戏音量 |
| isSoundsOn | int | 1 | 音效开关 |
| isdebug | int | 0 | 调试模式 |
| isPreLeft | int | 1 | 信息面板在左/右 |
| isPreListUp | int | 1 | 队列方向 向上/下 |
| MKeyAction | int | SHK_RET_APPEXIT_SHELLTOP | M 键动作：0=无效/1=最小化/2=关闭 |

读写入口：`SettingWnd::ReadSetting` ([L1468](file:///workspace/WaterPipe/WaterPipeMain.cpp#L1468)) / `SaveSetting` ([L1818](file:///workspace/WaterPipe/WaterPipeMain.cpp#L1818))。

### 12.2 [Ranking] 段

```
name_1   = "张三"        score_1 = 340
name_2   = "李四"        score_2 = 280
...
name_20  = "新玩家"      score_20 = 60
```

读入：`ReadSetting` ([L1514](file:///workspace/WaterPipe/WaterPipeMain.cpp#L1514)) 从 1 遍历到 20，读不到 score_N 就 break。
写入：`Ranking::SaveList(from,...)` ([Ranking.cpp#L218](file:///workspace/WaterPipe/Ranking.cpp#L218)) 从第 from 名起覆盖写入后续所有名次。

### 12.3 排行榜截图 BMP

进榜时 `Common::SaveBmp(save_mdc, "AppPath\ranking\rank_N.bmp")` 把终局画面存为 16-bit BMP。
`SaveList` 插入名次时，**对 bmp 文件从尾往前 MoveFile** 顺移 1 格（rank_{N}.bmp→rank_{N+1}.bmp），超 PIPE_RANKING_MAX 时 DeleteFile。
查看：`RankingBmp::OnInitDialog` 按 rankno 加载对应 bmp → 全屏显示。

---

## 13. 线程与并发模型

### 13.1 线程清单

| 线程 | 创建位置 | 入口函数 | 职责 |
|------|---------|---------|------|
| UI 主线程 | 框架 | MainApp::Init/MainWnd 消息泵 | 窗口消息、用户交互 |
| FinalThread | `Final()` [L2422](file:///workspace/WaterPipe/WaterPipeMain.cpp#L2422) | `FinalThread` | 注水总控：禁用 UI → ChackFirst → **波次循环**→ 音效切换 → 抓图 → 进榜弹窗 |
| FullAnimoThread ×N | FinalThread 波次循环 [L2708](file:///workspace/WaterPipe/WaterPipeMain.cpp#L2708) | `FullAnimoThread` | 单格 15 帧 AlphaBlend 动画，上限 `PIPE_ANIMO_MAX_THREADS=20` |

### 13.2 并发策略

- **波级串行 + 波内并行**：每波水流（now_chack_box 中所有方块）同时起 N 个动画线程，主线程对每个 HANDLE 依次 `WaitForSingleObject`（串行等待），保证本波全部完成后再进入 GetNext 推进下一波。
- **共享 DC 绘制无锁**：所有 FullAnimoThread 直接向全局 `hdc = GetWindowDC(main_hWnd)` 做 `AlphaBlend`，同步完全靠**各线程绘制区域恰好不重叠**（每格 UiButton_Image 的独立 RECT）—— 这是脆弱但高效的约定，一旦动画范围扩大会出现花屏。
- **跨线程 UI 操作**：FinalThread 内直接写 `theApp.m_MainWnd.m_Toolbar.SetEnable/SetTextColor/Invalidate`、`static_score.SetText` 等，未使用 PostMessage 回主线程，依赖 MZFC 近似的线程安全。
- **线程数据生命周期 Bug**：每波 `HeapAlloc(ThreadData)` 由 FinalThread 分配，FullAnimoThread 内不释放，循环结束后 FinalThread 只在最后做了一次 `HeapFree(pData)`（释放最后波最后一个结构），其余每波结构全部泄漏。

---

## 14. 构建与运行

### 14.1 构建环境

| 项 | 要求 |
|----|------|
| 操作系统 | Windows XP / 7（安装 WinCE SDK + M8SDK） |
| IDE | Visual Studio 2008（VC++9，vc80 工具集） |
| 目标平台 | Windows CE 5.0 / 6.0，ARMV4I 指令集 |
| SDK | Meizu M8 SDK（MZFC 框架，提供 mzfc_inc.h、sound.h、SettingApi.h、SettingApi.lib 等） |
| 解决方案 | [WaterPipe.sln](file:///workspace/WaterPipe.sln) → 工程 [WaterPipe.vcproj](file:///workspace/WaterPipe/WaterPipe.vcproj) |

### 14.2 构建输出

| 配置 | 路径 | 产物 |
|------|------|------|
| Debug | `/workspace/WaterPipe/M8SDK (ARMV4I)/Debug/` | WaterPipe.exe (约 300KB+), .pdb |
| Release | `/workspace/WaterPipe/M8SDK (ARMV4I)/Release/` | WaterPipe.exe |

### 14.3 在 M8 手机上安装与运行

1. 将 Release 版 `WaterPipe.exe` 复制到 M8 设备任意目录（推荐 `\Program Files\WaterPipe\`）
2. 将以下资源文件**与 exe 同目录放置**：
   - `Cover_bg.bmp`（封面背景）
   - `Main_bg.bmp`（主界面背景，可自换皮肤）
   - `Ranking_bg.bmp`（排行榜背景，可自换皮肤）
   - `key`（正版授权文件；缺失或不匹配 → 弹 CopyRight 页）
   - `ranking\` 子目录（首次运行会自动写入 `app.ini` 与 BMP 截图）
3. 在文件管理器点击 `WaterPipe.exe` 启动
4. 首次启动需输入注册：进入 CopyRight 页查看机器码 → 用 keygen 算号 → 写入同目录 key 文件 → 重启 exe

### 14.4 启动参数与注册表

- 无命令行参数
- 不写系统注册表，全部状态存同目录 `app.ini`
- 安装卸载 = 复制/删除整个目录（绿色）

---

## 15. 已知问题与代码异味

### 15.1 Bug

| # | 位置 | 严重 | 说明 |
|---|------|------|------|
| 1 | [DevInfo.cpp#L241-L244](file:///workspace/WaterPipe/DevInfo.cpp#L241) | 中 | SN 截断错位：`memcpy(SN,..)` 后写的是 `Result.IMEI[17]='\0'` 而非 `Result.SN[17]`，影响机器码生成稳定性 |
| 2 | [Common.cpp#L478](file:///workspace/WaterPipe/Common.cpp#L478) | 低 | `DirectoryExists` 逻辑错：`(FILE_ATTRIBUTE_DIRECTORY && code != 0)` 实际未校验目录属性 |
| 3 | [Common.cpp#L367](file:///workspace/WaterPipe/Common.cpp#L367) | 高 | `def.~DevInfo()` 显式析构后出作用域还会自动析构，**双重析构风险** |
| 4 | [WaterPipeMain.cpp#L2453](file:///workspace/WaterPipe/WaterPipeMain.cpp#L2453) | 高 | `NewGame()` 首次 `CloseHandle(hFinalThread)` 作用于未初始化全局 → 可能关闭随机句柄 / 崩溃 |
| 5 | [WaterPipeMain.cpp#L2426-L2430](file:///workspace/WaterPipe/WaterPipeMain.cpp#L2426) | 中 | `CreateThread` 失败时对 NULL CloseHandle 后直接 `ExitProcess(0)`，过于激进 |
| 6 | [WaterPipeMain.cpp#L1824](file:///workspace/WaterPipe/WaterPipeMain.cpp#L1824) | 中 | `SaveSetting` 将用户在系统音量滑块上的值**覆写** `dwOrgVolume` → 退出时无法恢复原音量（README v1.1 提到的修复点之一） |
| 7 | [WaterPipe.rc L82](file:///workspace/WaterPipe/WaterPipe.rc) | 低 | `IDR_WAVE_water_flow` 指向 `klunk.wav` 而非 `water_flow.wav`（资源标签错配） |
| 8 | DevInfo 解析循环 | 低 | 三个 `while(1){...; if((char)*p<=0) break;}` 先读后判，非 ASCII 边界易错截 |

### 15.2 内存泄漏

| # | 位置 | 说明 |
|---|------|------|
| 1 | [Ranking.cpp#L38](file:///workspace/WaterPipe/Ranking.cpp#L38) / L41 | `DrawItem` 每次 `new RECT` 两次，从不 delete |
| 2 | [WaterPipeMain.cpp#L132](file:///workspace/WaterPipe/WaterPipeMain.cpp#L132) | `ErrAnimo` 中 `new MemoryDC()` 只 `Unload()` 不 delete |
| 3 | [WaterPipeMain.cpp#L2714](file:///workspace/WaterPipe/WaterPipeMain.cpp#L2714) + L2790 | 每波 HeapAlloc 的 ThreadData，全流程只 HeapFree 最后一个 |
| 4 | [Common.cpp#L529](file:///workspace/WaterPipe/Common.cpp#L529) | `GetGuidString` `CoTaskMemFree(pstrGuid)` 被注释 |
| 5 | [Common.cpp#L90](file:///workspace/WaterPipe/Common.cpp#L90) / L94 | `delete lpBitmapBits`（CreateDIBSection 返回的位指针不应 delete，应 DeleteObject(directBmp)） |

### 15.3 死代码 / 冗余

| # | 内容 |
|---|------|
| 1 | [WaterPipeMain.h](file:///workspace/WaterPipe/WaterPipeMain.h) 全文件无人引用，且与 main.h 重复定义 |
| 2 | SoundsData/IPlayerCore/MIDI 背景乐：头包含、资源在、全不调用 |
| 3 | `getMacAddress()`：main.h 声明、Common.cpp 整段注释 |
| 4 | BoxAnimo/ErrThreadData/animo vector：声明但几乎未用 |
| 5 | main.h 第 6 列按钮 ID 宏 (PIPE_MAIN_BTN_1_6..) 未使用 |
| 6 | Common::Now() 返回空串（Date/Time 拼合被注释） |
| 7 | `MainApp::Init` L2573-L2635 整段 phone/TAPI/飞行模式 注释代码 |

### 15.4 拼写 / 硬编码 / 并发隐患

- 拼写：`HAFE_FRAME`(HALF)、`isInRepain`(Repaint)、`ChackFirst`(Check)、`now_chack_box`
- 分辨率强耦合：硬编码像素 390/90/395/430/350 等，仅适配 M8 720×480 工作区
- 并发：FullAnimoThread 共享 DC 无锁、FinalThread 跨线程直接操控 UI 控件
- 随机种子弱：`srand(年+月+日+时+分+秒)`，分钟/秒内重放完全同序列

---

## 16. 附录：常量速查

来源：[main.h](file:///workspace/WaterPipe/main.h)

| 宏 | 值 | 含义 |
|----|----|------|
| `PIPE_MAIN_NUM_IMG_H` | 5 | 网格列数 |
| `PIPE_MAIN_NUM_IMG_V` | 8 | 网格行数 |
| `PIPE_MAIN_NUM_IMG` | 45 | 可用方块总数（队列长度） |
| `PIPE_MAIN_NUM_RRELIST_IMG` | 5 | 预览可见方块数 |
| `PIPE_MAIN_FIRST_IMG_NO` | 25 | （未使用） |
| `PIPE_MAIN_SIZE_IMG` | 75 | 单格像素边长 |
| `PIPE_MAIN_LABLE_H` | 28 | 分数/剩余标签高度 |
| `PIPE_MAIN_SPAN_OUT / SPAN_IN / SPAN_IMG` | 5 / 15 / 1 | 外边距 / 内边距 / 格间距 |
| `PIPE_HEIGHT_CAPTION` | 60 | 标题条高度 |
| `PIPE_RANKING_MAX` | 20 | 排行榜最大名次 |
| `PIPE_RANKING_HIGHT` | 55 | 排行榜每项像素高 |
| `PIPE_ANIMO_MAX_THREADS` | 20 | 每波动画线程上限 |
| `PIPE_ANIMO_FRAME` | 15 | 单格动画总帧数 |
| `PIPE_ANIMO_HAFE_FRAME` | 8 | 半格分界帧（拼写 HALF） |
| `PIPE_ANIMO_FRAME_BETWEEN` | 100 | 每帧 Sleep 毫秒 |

---

*本文档基于 WaterPipe 1.5 源码静态分析生成。*
