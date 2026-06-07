; =============================================================
; Minecraft Launcher - Inno Setup Install Script
; 我的世界启动器安装程序脚本
; =============================================================
; 使用方法:
;   1. 将项目以 Release 模式编译 (生成 bin\Release 目录)
;   2. 使用 Inno Setup Compiler 打开此文件 (.iss)
;   3. 点击 Build -> Compile 生成安装包
;   4. 安装包将输出到 Output 目录
; =============================================================

#define MyAppName      "我的世界启动器"
#define MyAppVersion   "1.0.0"
#define MyAppPublisher "C-Luncher Team"
#define MyAppURL       "https://github.com/transform1155/C-Luncher"
#define MyAppExeName   "WindowsFormsApp2.exe"

[Setup]
; 应用基本信息
AppId={{E1F5A3B8-4C72-4D5A-9A2E-8F7C6D5E4B3C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}

; 安装目录设置
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}

; 输出设置
OutputBaseFilename=MinecraftLauncher-{#MyAppVersion}-Setup
OutputDir=..\..\Output
Compression=lzma2/ultra
SolidCompression=yes
WizardStyle=modern

; 安装选项
AllowNoIcons=no
AlwaysShowDirOnReadyPage=yes
AlwaysShowGroupOnReadyPage=yes
ArchitecturesAllowed=x64 x86
ArchitecturesInstallIn64BitMode=x64
DisableProgramGroupPage=no
DisableReadyPage=no
DisableWelcomePage=no

; 语言
Languages=chinesesimplified

; 界面
SetupIconFile=..\..\resources\installer.ico
WizardImageFile=WizardImage.bmp
WizardSmallImageFile=WizardSmallImage.bmp

; 权限
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

; 预发布信息 (可选)
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription={#MyAppName} 安装程序
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}

[Languages]
Name: "chinesesimplified"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"

[Tasks]
Name: "desktopicon"; Description: "创建桌面快捷方式"; GroupDescription: "附加图标:"; Flags: unchecked
Name: "quicklaunchicon"; Description: "创建快速启动栏快捷方式"; GroupDescription: "附加图标:"; Flags: unchecked; OnlyBelowVersion: 0,6.1
Name: "autostart"; Description: "开机自动启动"; GroupDescription: "自动启动:"; Flags: unchecked

[Files]
; 主程序文件
Source: "..\WindowsFormsApp2\bin\Release\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\WindowsFormsApp2\bin\Release\WindowsFormsApp2.exe.config"; DestDir: "{app}"; Flags: ignoreversion

; NuGet 依赖库文件
Source: "..\WindowsFormsApp2\bin\Release\RestSharp.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\WindowsFormsApp2\bin\Release\System.Text.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\WindowsFormsApp2\bin\Release\System.Text.Encodings.Web.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\WindowsFormsApp2\bin\Release\System.IO.Pipelines.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\WindowsFormsApp2\bin\Release\Microsoft.Bcl.AsyncInterfaces.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\WindowsFormsApp2\bin\Release\System.Memory.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\WindowsFormsApp2\bin\Release\System.Threading.Tasks.Extensions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\WindowsFormsApp2\bin\Release\System.Runtime.CompilerServices.Unsafe.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\WindowsFormsApp2\bin\Release\System.Numerics.Vectors.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\WindowsFormsApp2\bin\Release\System.Buffers.dll"; DestDir: "{app}"; Flags: ignoreversion

; 文档文件
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion

; 整个发布目录 (如果不确定具体文件，可以使用这两行替代上面的单独文件声明)
; Source: "..\WindowsFormsApp2\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"
Name: "{group}\卸载 {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{group}\{#MyAppName} 帮助"; Filename: "{app}\README.md"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Registry]
; 注册应用程序到控制面板中的卸载列表 (可选，Inno Setup 自动处理)
; 但我们可以添加一些自定义注册表项

; 注册应用程序版本号
Root: HKLM; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "Version"; ValueData: "{#MyAppVersion}"; Flags: uninsdeletevalue
Root: HKLM; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"; Flags: uninsdeletevalue
Root: HKLM; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "InstallDate"; ValueData: "{code:GetDateString}"; Flags: uninsdeletevalue

[Run]
; 安装完成后可选运行程序
Filename: "{app}\{#MyAppExeName}"; Description: "运行 {#MyAppName}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; 卸载时删除用户数据文件 (可选)
Type: filesandordirs; Name: "{userappdata}\{#MyAppName}"
Type: filesandordirs; Name: "{app}\game_files"

[Code]
function GetDateString(Value: String): String;
var
  Year, Month, Day: String;
begin
  GetDate(Year, Month, Day);
  Result := Year + '-' + Month + '-' + Day;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ErrorCode: Integer;
begin
  if CurStep = ssPostInstall then
  begin
    { 安装完成后创建用户数据目录 }
    if not DirExists(ExpandConstant('{userappdata}\{#MyAppName}')) then
    begin
      CreateDir(ExpandConstant('{userappdata}\{#MyAppName}'));
    end;
  end;
end;
