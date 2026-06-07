@echo off
REM =============================================================
REM Minecraft Launcher - Build and Installer Script (Batch)
REM 我的世界启动器 - 构建与安装程序生成脚本 (批处理版)
REM =============================================================
REM 使用方法:
REM   1. 双击此文件 (build-installer.bat)
REM   2. 或在命令行中执行: build-installer.bat
REM
REM 要求:
REM   - Visual Studio 2019/2022 (含 .NET Framework 4.7.2)
REM   - Inno Setup (ISCC.exe) 已安装
REM =============================================================

setlocal

echo ========================================
echo   Minecraft Launcher 构建脚本 (批处理)
echo ========================================
echo.

REM ------------------------------------------------------------
REM 配置
REM ------------------------------------------------------------
set CONFIGURATION=Release
set PLATFORM=AnyCPU
set SKIP_BUILD=0
set SKIP_INSTALLER=0

set PROJECT_DIR=%~dp0WindowsFormsApp2
set PROJECT_FILE=%PROJECT_DIR%\WindowsFormsApp2.csproj
set BIN_DIR=%PROJECT_DIR%\bin\%CONFIGURATION%
set INSTALLER_DIR=%~dp0installer
set OUTPUT_DIR=%~dp0Output
set INSTALLER_SCRIPT=%INSTALLER_DIR%\MinecraftLauncher.iss

echo [配置]
echo   项目文件: %PROJECT_FILE%
echo   配置模式: %CONFIGURATION% ^| %PLATFORM%
echo   输出目录: %OUTPUT_DIR%
echo.

REM ------------------------------------------------------------
REM 步骤 1: 清理输出目录
REM ------------------------------------------------------------
echo [步骤 1/4] 清理输出目录...
if exist "%OUTPUT_DIR%" (
    echo   清理: %OUTPUT_DIR%
) else (
    mkdir "%OUTPUT_DIR%"
    echo   创建目录: %OUTPUT_DIR%
)
echo   完成
echo.

REM ------------------------------------------------------------
REM 步骤 2: 构建项目
REM ------------------------------------------------------------
if "%SKIP_BUILD%"=="1" goto :skip_build

echo [步骤 2/4] 构建项目...

REM 尝试查找 MSBuild
set MSBUILD_PATH=

REM 使用 vswhere 查找 Visual Studio 安装路径
set VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe
if exist "%VSWHERE%" (
    for /f "usebackq delims=" %%i in (`"%VSWHERE%" -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) do (
        set VS_INSTALL=%%i
    )
    if not "%VS_INSTALL%"=="" (
        if exist "%VS_INSTALL%\MSBuild\Current\Bin\MSBuild.exe" (
            set MSBUILD_PATH=%VS_INSTALL%\MSBuild\Current\Bin\MSBuild.exe
        ) else if exist "%VS_INSTALL%\MSBuild\17.0\Bin\MSBuild.exe" (
            set MSBUILD_PATH=%VS_INSTALL%\MSBuild\17.0\Bin\MSBuild.exe
        ) else if exist "%VS_INSTALL%\MSBuild\16.0\Bin\MSBuild.exe" (
            set MSBUILD_PATH=%VS_INSTALL%\MSBuild\16.0\Bin\MSBuild.exe
        )
    )
)

REM 如果 vswhere 没找到，尝试其他常见位置
if "%MSBUILD_PATH%"=="" (
    if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
        set MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe
    ) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
        set MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe
    )
)

if "%MSBUILD_PATH%"=="" (
    echo   警告: 未找到 MSBuild，跳过构建步骤
    echo   请手动在 Visual Studio 中以 Release 模式构建项目
    echo.
    goto :verify_build
)

echo   使用 MSBuild: %MSBUILD_PATH%
echo   构建项目文件: %PROJECT_FILE%
echo.

REM 执行构建
"%MSBUILD_PATH%" "%PROJECT_FILE%" /t:Clean,Build /p:Configuration=%CONFIGURATION% /p:Platform=%PLATFORM% /nologo /v:minimal
if errorlevel 1 (
    echo   构建失败
    pause
    exit /b 1
)

echo   构建成功
goto :verify_build

:skip_build
echo [步骤 2/4] 跳过构建
echo.

REM ------------------------------------------------------------
REM 步骤 3: 验证构建产物
REM ------------------------------------------------------------
:verify_build
echo [步骤 3/4] 验证构建产物...

if not exist "%BIN_DIR%\WindowsFormsApp2.exe" (
    echo   错误: 未找到构建产物 WindowsFormsApp2.exe
    echo   请先在 Visual Studio 中构建项目 (Release 模式)
    pause
    exit /b 1
)

echo   构建产物清单:
for %%f in ("%BIN_DIR%\*.*") do (
    echo     - %%~nxf
)
echo   验证通过
echo.

REM ------------------------------------------------------------
REM 步骤 4: 生成安装程序
REM ------------------------------------------------------------
if "%SKIP_INSTALLER%"=="1" goto :skip_installer

echo [步骤 4/4] 生成安装程序...

REM 查找 ISCC.exe
set ISCC_PATH=

REM 从 PATH 中查找
for /f "usebackq delims=" %%i in (`where ISCC.exe 2^>nul`) do (
    set ISCC_PATH=%%i
    goto :found_iscc
)

REM 常见安装位置
:search_iscc_locations
if exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" (
    set ISCC_PATH=C:\Program Files (x86)\Inno Setup 6\ISCC.exe
    goto :found_iscc
)
if exist "C:\Program Files (x86)\Inno Setup 5\ISCC.exe" (
    set ISCC_PATH=C:\Program Files (x86)\Inno Setup 5\ISCC.exe
    goto :found_iscc
)
if exist "C:\Program Files\Inno Setup 6\ISCC.exe" (
    set ISCC_PATH=C:\Program Files\Inno Setup 6\ISCC.exe
    goto :found_iscc
)
if exist "C:\Program Files\Inno Setup 5\ISCC.exe" (
    set ISCC_PATH=C:\Program Files\Inno Setup 5\ISCC.exe
    goto :found_iscc
)

:found_iscc
if "%ISCC_PATH%"=="" (
    echo   警告: 未找到 Inno Setup 编译器 (ISCC.exe)
    echo.
    echo   请从以下地址下载并安装 Inno Setup:
    echo     https://jrsoftware.org/isdl.php
    echo.
    echo   安装后，重新运行此脚本
    echo   或在 Inno Setup Compiler 中手动打开 installer\MinecraftLauncher.iss
    echo.
    pause
    exit /b 1
)

echo   使用 ISCC: %ISCC_PATH%
echo   安装脚本: %INSTALLER_SCRIPT%
echo.

REM 执行 ISCC 编译
"%ISCC_PATH%" /O"%OUTPUT_DIR%" /Q "%INSTALLER_SCRIPT%"
if errorlevel 1 (
    echo   安装程序生成失败
    echo   请检查 Inno Setup 脚本
    pause
    exit /b 1
)

echo   安装程序生成成功!
echo   安装包目录: %OUTPUT_DIR%
echo.
goto :done

:skip_installer
echo [步骤 4/4] 跳过安装程序生成
echo.

:done
echo ========================================
echo   构建完成!
echo ========================================
echo.

REM 列出生成的文件
if exist "%OUTPUT_DIR%" (
    echo   输出目录中的文件:
    dir /b "%OUTPUT_DIR%"
)

echo.
echo   按任意键退出...
pause >nul

endlocal
