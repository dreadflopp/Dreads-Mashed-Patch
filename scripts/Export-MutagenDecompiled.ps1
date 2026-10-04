param(
    [string]$AssetsFile = "DreadsMashedPatch/obj/project.assets.json",
    [string]$OutputRoot = "DecompiledMutagen",
    [string]$Framework = "net10.0",
    [string]$DecompilerVersion = "9.1.0.7988"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Split-Path -Parent $PSScriptRoot)).ProviderPath
$toolDirectory = Join-Path $repoRoot ".tools"
$onWindows = [System.IO.Path]::DirectorySeparatorChar -eq '\'
$toolName = if ($onWindows) { "ilspycmd.exe" } else { "ilspycmd" }
$toolPath = Join-Path $toolDirectory $toolName
$outputPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $OutputRoot))
$repoPrefix = $repoRoot.TrimEnd([System.IO.Path]::DirectorySeparatorChar) +
    [System.IO.Path]::DirectorySeparatorChar
$pathComparison = if ($onWindows) { [StringComparison]::OrdinalIgnoreCase } else { [StringComparison]::Ordinal }
if (-not $outputPath.StartsWith($repoPrefix, $pathComparison)) {
    throw "Output path must remain inside the repository: $outputPath"
}
# Do not follow directory links when replacing generated sources.
for ($parent = $outputPath; $parent -ne $repoRoot; $parent = Split-Path -Parent $parent) {
    if ((Test-Path -LiteralPath $parent) -and
        ((Get-Item -LiteralPath $parent -Force).Attributes -band [System.IO.FileAttributes]::ReparsePoint)) {
        throw "Output path must not traverse a directory link: $parent"
    }
}

Push-Location $repoRoot
try {
    if (-not (Test-Path -LiteralPath $AssetsFile)) {
        throw "Assets file not found: $AssetsFile. Run dotnet restore DreadsMashedPatch/DreadsMashedPatch.csproj first."
    }
    $assetsPath = (Resolve-Path $AssetsFile).ProviderPath

    $assets = Get-Content -Path $assetsPath -Raw | ConvertFrom-Json
    $packageRoots = @($assets.packageFolders.PSObject.Properties.Name)
    $libraries = @(
        $assets.libraries.PSObject.Properties |
        Where-Object { $_.Value.path -like "mutagen.bethesda*" } |
        Sort-Object Name
    )
    if ($libraries.Count -eq 0) {
        throw "No Mutagen.Bethesda libraries were found in $AssetsFile. Run a restore/build first."
    }

    $assemblies = @()
    foreach ($library in $libraries) {
        $dllFiles = @(
            $library.Value.files |
            Where-Object { $_ -like "lib/$Framework/*.dll" -and $_ -notlike "*.resources.dll" }
        )
        if ($dllFiles.Count -eq 0) {
            throw "No $Framework assemblies found in $($library.Name). Select an available framework with -Framework. Existing output was not changed."
        }

        foreach ($dllRelativePath in $dllFiles) {
            $dllPath = $null
            foreach ($packageRoot in $packageRoots) {
                $candidate = Join-Path (Join-Path $packageRoot $library.Value.path) $dllRelativePath
                if (Test-Path -LiteralPath $candidate) {
                    $dllPath = $candidate
                    break
                }
            }
            if (-not $dllPath) {
                throw "Assembly not found in restored package folders: $($library.Name)/$dllRelativePath. Restore the project on this machine first."
            }
            $assemblies += $dllPath
        }
    }
    $referenceDirectories = @($assemblies | ForEach-Object { Split-Path -Parent $_ } | Sort-Object -Unique)

    if (-not (Test-Path -LiteralPath $toolPath)) {
        New-Item -ItemType Directory -Path $toolDirectory -Force | Out-Null
        dotnet tool install ilspycmd --tool-path $toolDirectory --version $DecompilerVersion | Out-Host
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to install ilspycmd $DecompilerVersion."
        }
    }
    & $toolPath --version | Out-Host
    if ($LASTEXITCODE -ne 0) {
        throw "ilspycmd could not start. The pinned version requires the .NET 8 runtime (in addition to the project's .NET 10 SDK)."
    }

    if (Test-Path -LiteralPath $outputPath) {
        Remove-Item -LiteralPath $outputPath -Recurse -Force
    }
    New-Item -ItemType Directory -Path $outputPath -Force | Out-Null

    foreach ($dllPath in $assemblies) {
        $assemblyName = [System.IO.Path]::GetFileNameWithoutExtension($dllPath)
        $assemblyOutputPath = Join-Path $outputPath $assemblyName
        $decompilerArguments = @(
            "--disable-updatecheck",
            "--nested-directories",
            "-p",
            "-o",
            $assemblyOutputPath
        )

        foreach ($referenceDirectory in $referenceDirectories) {
            $decompilerArguments += @("-r", $referenceDirectory)
        }

        $decompilerArguments += $dllPath

        Write-Host "Decompiling $assemblyName"
        & $toolPath @decompilerArguments | Out-Host

        if ($LASTEXITCODE -ne 0) {
            throw "Decompilation failed for $assemblyName with exit code $LASTEXITCODE."
        }

        if (-not (Test-Path $assemblyOutputPath)) {
            throw "Decompilation produced no output folder for $assemblyName."
        }

        if (-not (Get-ChildItem -Path $assemblyOutputPath -Force | Select-Object -First 1)) {
            throw "Decompilation produced an empty output folder for $assemblyName."
        }
    }
}
finally {
    Pop-Location
}
