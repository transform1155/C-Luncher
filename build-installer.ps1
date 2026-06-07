# =============================================================
# Minecraft Launcher - Build and Installer Script (PowerShell)
# 我的世界启动器 - 构建与安装程序生成脚本
# =============================================================
# 使用方法:
#   1. 打开 PowerShell
#   2. 进入项目根目录: cd C:\Users\Administrator\Documents\GitHub\C-Luncher
#   3. 执行: .\build-installer.ps1
#
# 要求:
#   - Visual Studio 2019/2022 (含 .NET Framework 4.7.2)
#   - 或 .NET SDK (如果是 .NET Core 项目)
#   - Inno Setup (ISCC.exe) 已安装
# =============================================================

param(
    [string]$Configuration = "Release",
    [string]$Platform = "AnyCPU",
    [switch]$SkipBuild = $false,
    [switch]$SkipInstaller = $false,
    [string]$InnoSetupPath = ""
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Minecraft Launcher 构建脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ------------------------------------------------------------
# 1. 路径配置
# ------------------------------------------------------------
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = $scriptDir
$projectDir = Join-Path $projectRoot "WindowsFormsApp2"
$projectFile = Join-Path $projectDir "WindowsFormsApp2.csproj"
$solutionFile = Join-Path $projectDir "WindowsFormsApp2.sln"
$binDir = Join-Path $projectDir "bin\$Configuration"
$installerDir = Join-Path $projectRoot "installer"
$outputDir = Join-Path $projectRoot "Output"
$installerScript = Join-Path $installerDir "MinecraftLauncher.iss"

Write-Host "[配置]" -ForegroundColor Yellow
Write-Host "  项目根目录: $projectRoot" -ForegroundColor Gray
Write-Host "  项目文件: $projectFile" -ForegroundColor Gray
Write-Host "  配置模式: $Configuration | $Platform" -ForegroundColor Gray
Write-Host "  输出目录: $outputDir" -ForegroundColor Gray
Write-Host ""

# ------------------------------------------------------------
# 2. 清理并创建输出目录
# ------------------------------------------------------------
Write-Host "[步骤 1/4] 清理输出目录..." -ForegroundColor Yellow
if (Test-Path $binDir) {
    Write-Host "  清理: $binDir" -ForegroundColor Gray
    Remove-Item -Path "$binDir\*" -Recurse -Force -ErrorAction SilentlyContinue
}
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir | Out-Null
    Write-Host "  创建目录: $outputDir" -ForegroundColor Gray
}
Write-Host "  完成" -ForegroundColor Green
Write-Host ""

# ------------------------------------------------------------
# 3. 构建项目
# ------------------------------------------------------------
if (-not $SkipBuild) {
    Write-Host "[步骤 2/4] 构建项目..." -ForegroundColor Yellow

    # 尝试查找 MSBuild
    $msbuildPath = ""
    $vsWherePath = Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio\Installer\vswhere.exe"

    if (Test-Path $vsWherePath) {
        # 使用 vswhere 查找最新的 VS 安装
        try {
            $vsInstallPath = & $vsWherePath -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
            if ($vsInstallPath) {
                $candidates = @(
                    (Join-Path $vsInstallPath "MSBuild\Current\Bin\MSBuild.exe"),
                    (Join-Path $vsInstallPath "MSBuild\17.0\Bin\MSBuild.exe"),
                    (Join-Path $vsInstallPath "MSBuild\16.0\Bin\MSBuild.exe"),
                    (Join-Path $vsInstallPath "MSBuild\15.0\Bin\MSBuild.exe")
                )
                foreach ($candidate in $candidates) {
                    if (Test-Path $candidate) {
                        $msbuildPath = $candidate
                        break
                    }
                }
            }
        } catch {
            Write-Host "  vswhere 执行失败，尝试其他方式..." -ForegroundColor Gray
        }
    }

    # 如果 vswhere 没找到，尝试其他常见位置
    if ([string]::IsNullOrEmpty($msbuildPath)) {
        $candidates = @(
            "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
            "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
            "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
            "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
            "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe",
            "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
        )
        foreach ($candidate in $candidates) {
            if (Test-Path $candidate) {
                $msbuildPath = $candidate
                break
            }
        }
    }

    if ([string]::IsNullOrEmpty($msbuildPath)) {
        Write-Host "  警告: 未找到 MSBuild，跳过构建步骤" -ForegroundColor Red
        Write-Host "  请手动在 Visual Studio 中以 Release 模式构建项目" -ForegroundColor Red
        Write-Host ""
    } else {
        Write-Host "  使用 MSBuild: $msbuildPath" -ForegroundColor Gray
        Write-Host "  构建项目文件: $projectFile" -ForegroundColor Gray
        Write-Host ""

        # 执行 MSBuild 构建
        $buildArgs = @(
            "`"$projectFile`"",
            "/t:Clean,Build",
            "/p:Configuration=$Configuration",
            "/p:Platform=$Platform",
            "/nologo",
            "/v:minimal"
        )

        $buildResult = Start-Process -FilePath $msbuildPath -ArgumentList $buildArgs -NoNewWindow -Wait -PassThru -RedirectStandardOutput "build.log" -RedirectStandardError "build.err.log"

        if ($buildResult.ExitCode -ne 0) {
            Write-Host "  构建失败，退出代码: $($buildResult.ExitCode)" -ForegroundColor Red
            Write-Host "  查看 build.log 和 build.err.log 了解详情" -ForegroundColor Red
            exit 1
        }

        # 检查构建产物
        if (-not (Test-Path (Join-Path $binDir "WindowsFormsApp2.exe"))) {
            Write-Host "  错误: 未找到构建产物 WindowsFormsApp2.exe" -ForegroundColor Red
            exit 1
        }

        $fileCount = (Get-ChildItem $binDir -File).Count
        Write-Host "  构建成功! 生成了 $fileCount 个文件" -ForegroundColor Green
    }
} else {
    Write-Host "[步骤 2/4] 跳过构建 (--SkipBuild)" -ForegroundColor Yellow
}
Write-Host ""

# ------------------------------------------------------------
# 4. 验证构建产物
# ------------------------------------------------------------
Write-Host "[步骤 3/4] 验证构建产物..." -ForegroundColor Yellow

if (-not (Test-Path $binDir)) {
    Write-Host "  错误: 构建目录不存在: $binDir" -ForegroundColor Red
    Write-Host "  请先在 Visual Studio 中构建项目 (Release 模式)" -ForegroundColor Red
    exit 1
}

$requiredFiles = @(
    "WindowsFormsApp2.exe",
    "WindowsFormsApp2.exe.config"
)

$missingFiles = @()
foreach ($file in $requiredFiles) {
    $filePath = Join-Path $binDir $file
    if (-not (Test-Path $filePath)) {
        $missingFiles += $file
    }
}

if ($missingFiles.Count -gt 0) {
    Write-Host "  缺失以下文件:" -ForegroundColor Red
    foreach ($file in $missingFiles) {
        Write-Host "    - $file" -ForegroundColor Red
    }
    exit 1
}

$fileList = Get-ChildItem $binDir -File
Write-Host "  构建产物清单 ($($fileList.Count) 个文件):" -ForegroundColor Green
foreach ($file in $fileList) {
    $sizeKB = [math]::Round($file.Length / 1KB, 2)
    Write-Host "    - $($file.Name) ($sizeKB KB)" -ForegroundColor Gray
}
Write-Host "  验证通过" -ForegroundColor Green
Write-Host ""

# ------------------------------------------------------------
# 5. 生成安装程序
# ------------------------------------------------------------
if (-not $SkipInstaller) {
    Write-Host "[步骤 4/4] 生成安装程序..." -ForegroundColor Yellow

    # 查找 ISCC.exe (Inno Setup 编译器)
    $isccPath = ""

    # 优先使用用户指定的路径
    if (-not [string]::IsNullOrEmpty($InnoSetupPath)) {
        if (Test-Path $InnoSetupPath) {
            $isccPath = $InnoSetupPath
        } else {
            Write-Host "  指定的 InnoSetup 路径无效: $InnoSetupPath" -ForegroundColor Red
        }
    }

    # 从 PATH 环境变量中查找
    if ([string]::IsNullOrEmpty($isccPath)) {
        $pathFromEnv = Get-Command ISCC.exe -ErrorAction SilentlyContinue
        if ($pathFromEnv) {
            $isccPath = $pathFromEnv.Source
        }
    }

    # 常见安装位置
    if ([string]::IsNullOrEmpty($isccPath)) {
        $innoCandidates = @(
            "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
            "C:\Program Files (x86)\Inno Setup 5\ISCC.exe",
            "C:\Program Files\Inno Setup 6\ISCC.exe",
            "C:\Program Files\Inno Setup 5\ISCC.exe",
            "C:\InnoSetup\ISCC.exe"
        )
        foreach ($candidate in $innoCandidates) {
            if (Test-Path $candidate) {
                $isccPath = $candidate
                break
            }
        }
    }

    if ([string]::IsNullOrEmpty($isccPath)) {
        Write-Host "  警告: 未找到 Inno Setup 编译器 (ISCC.exe)" -ForegroundColor Red
        Write-Host ""
        Write-Host "  请从以下地址下载并安装 Inno Setup:" -ForegroundColor Yellow
        Write-Host "    https://jrsoftware.org/isdl.php" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "  安装后，可使用以下方式之一:" -ForegroundColor Yellow
        Write-Host "    1. 将 Inno Setup 安装目录加入系统 PATH" -ForegroundColor Gray
        Write-Host "    2. 使用 -InnoSetupPath 参数指定 ISCC.exe 完整路径" -ForegroundColor Gray
        Write-Host "    3. 在 Inno Setup Compiler 中手动打开 installer/MinecraftLauncher.iss" -ForegroundColor Gray
        Write-Host ""
        exit 1
    }

    Write-Host "  使用 ISCC: $isccPath" -ForegroundColor Gray
    Write-Host "  安装脚本: $installerScript" -ForegroundColor Gray
    Write-Host ""

    # 执行 ISCC 编译
    $isccArgs = @(
        "/O`"$outputDir`"",
        "/Q",
        "`"$installerScript`""
    )

    $isccResult = Start-Process -FilePath $isccPath -ArgumentList $isccArgs -NoNewWindow -Wait -PassThru -RedirectStandardOutput "iscc.log" -RedirectStandardError "iscc.err.log"

    if ($isccResult.ExitCode -ne 0) {
        Write-Host "  安装程序生成失败，退出代码: $($isccResult.ExitCode)" -ForegroundColor Red
        Write-Host "  查看 iscc.log 和 iscc.err.log 了解详情" -ForegroundColor Red
        Write-Host "  错误信息:" -ForegroundColor Red
        if (Test-Path "iscc.err.log") {
            Get-Content "iscc.err.log" | ForEach-Object { Write-Host "    $_" -ForegroundColor Red }
        }
        exit 1
    }

    # 检查生成的安装包
    $installerFiles = Get-ChildItem $outputDir -Filter "*.exe"
    if ($installerFiles.Count -eq 0) {
        Write-Host "  警告: 未在输出目录找到 .exe 安装文件" -ForegroundColor Red
        Write-Host "  请检查 iscc.log 了解编译详情" -ForegroundColor Red
        exit 1
    }

    Write-Host ""
    Write-Host "  安装程序生成成功!" -ForegroundColor Green
    Write-Host "  安装包目录: $outputDir" -ForegroundColor Gray
    foreach ($file in $installerFiles) {
        $sizeMB = [math]::Round($file.Length / 1MB, 2)
        Write-Host "    - $($file.Name) ($sizeMB MB)" -ForegroundColor Cyan
    }

} else {
    Write-Host "[步骤 4/4] 跳过安装程序生成 (--SkipInstaller)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  构建完成!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
