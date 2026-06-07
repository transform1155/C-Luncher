# Installer 目录说明

本目录包含 Minecraft 启动器的 Windows 安装程序脚本和相关资源。

## 生成安装程序的方式

### 方式 1：使用自动化脚本（推荐）

在项目根目录执行：

```powershell
# PowerShell 版本（功能完整，推荐）
.\build-installer.ps1

# 批处理版本（双击即可运行）
.\build-installer.bat
```

脚本会自动完成以下步骤：

1. 查找并调用 MSBuild 编译项目（Release 模式）
2. 验证构建产物是否完整
3. 查找并调用 Inno Setup 编译器 (ISCC.exe)
4. 生成安装包到 `Output` 目录

脚本参数（PowerShell 版）：

```powershell
.\build-installer.ps1 -Configuration Release -Platform AnyCPU
.\build-installer.ps1 -SkipBuild          # 跳过构建，直接用现有产物
.\build-installer.ps1 -SkipInstaller      # 跳过安装程序生成
.\build-installer.ps1 -InnoSetupPath "C:\Path\To\ISCC.exe"
```

### 方式 2：手动编译（适用于调试安装脚本）

1. 在 Visual Studio 中打开 `WindowsFormsApp2.sln`
2. 选择 **Release** 配置，**AnyCPU** 平台，点击 **生成**（Build）
3. 下载并安装 [Inno Setup](https://jrsoftware.org/isdl.php)
4. 打开 **Inno Setup Compiler**，加载 `MinecraftLauncher.iss`
5. 点击 **Build → Compile** 或按 **Ctrl+F9**
6. 安装包将生成到 `..\Output` 目录

## 文件说明

| 文件 | 说明 |
|------|------|
| `MinecraftLauncher.iss` | Inno Setup 安装脚本，定义了安装程序的完整配置 |
| `WizardImage.bmp` | （可选）安装向导左侧大图 (164x314px) |
| `WizardSmallImageFile.bmp` | （可选）安装向导右上角小图 (55x58px) |
| `installer.ico` | （可选）安装程序图标 (256x256px, ICO 格式) |

## 安装脚本可配置项

编辑 `MinecraftLauncher.iss` 顶部的 `#define` 部分：

```
#define MyAppName      "我的世界启动器"
#define MyAppVersion   "1.0.0"
#define MyAppPublisher "C-Luncher Team"
#define MyAppURL       "https://github.com/transform1155/C-Luncher"
#define MyAppExeName   "WindowsFormsApp2.exe"
```

其他可配置选项：

| 选项 | 位置 | 说明 |
|------|------|------|
| 安装目录 | `DefaultDirName` | 默认 `{autopf}\Minecraft Launcher` |
| 压缩算法 | `Compression` | 默认 `lzma2/ultra`（最大压缩） |
| 权限要求 | `PrivilegesRequired` | 默认 `lowest`（不需要管理员权限） |
| 架构限制 | `ArchitecturesAllowed` | 默认 x86 + x64 |
| 语言 | `Languages` | 默认简体中文 |

## 安装程序特性

- 现代化安装向导界面（WizardStyle=modern）
- 简体中文界面
- 可选桌面快捷方式
- 可选快速启动栏快捷方式
- 可选开机自动启动
- 开始菜单快捷方式和卸载入口
- 注册表记录（版本号、安装路径、安装日期）
- 用户数据目录自动创建
- 卸载时清理用户数据

## 构建产物依赖

安装程序将打包以下文件：

- `WindowsFormsApp2.exe` - 主程序
- `WindowsFormsApp2.exe.config` - 应用配置
- `RestSharp.dll` - REST API 客户端
- `System.Text.Json.dll` - JSON 处理
- `System.Text.Encodings.Web.dll` - 文本编码
- `System.IO.Pipelines.dll` - 管道 I/O
- `Microsoft.Bcl.AsyncInterfaces.dll` - 异步接口
- `System.Memory.dll` - 内存类型
- `System.Threading.Tasks.Extensions.dll` - 任务扩展
- `System.Runtime.CompilerServices.Unsafe.dll` - 不安全代码
- `System.Numerics.Vectors.dll` - 数值向量
- `System.Buffers.dll` - 缓冲区
- `README.md` - 项目文档

## 常见问题

### Q: 找不到 MSBuild.exe

确保已安装 Visual Studio 2019 或 2022，包含 `.NET 桌面开发` 工作负载。

### Q: 找不到 ISCC.exe

从 [https://jrsoftware.org/isdl.php](https://jrsoftware.org/isdl.php) 下载并安装 Inno Setup，或在安装后将其加入系统 PATH。

### Q: 构建成功但找不到 .exe

检查构建配置是否为 **Release**。在 Visual Studio 工具栏的配置下拉框中选择 **Release**，然后重新生成。

### Q: 生成的安装包体积太大

- 减小压缩级别：`Compression=lzma2/normal`
- 只包含必要的 DLL
- 考虑使用 .NET Core 的单文件发布（需要迁移项目）

### Q: 如何添加自定义图标

1. 准备一个 256x256 的 ICO 文件
2. 将其放到 `installer\installer.ico`
3. Inno Setup 会自动使用它作为安装程序图标
4. 同时修改 `[Icons]` 部分的 `IconFilename`

### Q: 如何添加自定义的欢迎图片

1. 准备两张 BMP 图片：
   - WizardImage.bmp (164x314 像素)
   - WizardSmallImage.bmp (55x58 像素)
2. 放到 `installer` 目录
3. Inno Setup 会自动使用

## 系统要求

**安装环境（构建安装程序的电脑）：**
- Windows 7 或更高版本
- Visual Studio 2019/2022（或 Build Tools）
- Inno Setup 5.x 或 6.x

**目标用户环境（运行安装程序的电脑）：**
- Windows 7 SP1 或更高版本
- .NET Framework 4.7.2（Windows 10 1803+ / Windows 11 已预装）
- 至少 2GB RAM
- 至少 200MB 可用磁盘空间
