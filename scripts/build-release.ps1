[CmdletBinding()]
param(
    [string]$SptPath,
    [switch]$NoRestore
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$projectPath = Join-Path $repoRoot 'src\SPT.Ready\SPT.Ready.csproj'
$outputDll = Join-Path $repoRoot 'src\SPT.Ready\bin\Release\netstandard2.1\SPT.Ready.dll'
$artifactsRoot = Join-Path $repoRoot 'artifacts'
$packageRoot = Join-Path $artifactsRoot 'package'
$pluginRoot = Join-Path $packageRoot 'BepInEx\plugins\SPT.Ready'
$distRoot = Join-Path $repoRoot 'dist'
$archivePath = Join-Path $distRoot 'SPT-Ready-4.1.0.zip'

function Assert-WorkspaceChild {
    param([Parameter(Mandatory)][string]$Path)

    $resolvedPath = [System.IO.Path]::GetFullPath($Path)
    $workspacePrefix = $repoRoot.TrimEnd([System.IO.Path]::DirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar

    if (!$resolvedPath.StartsWith($workspacePrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to modify a path outside the repository: $resolvedPath"
    }

    return $resolvedPath
}

$packageRoot = Assert-WorkspaceChild -Path $packageRoot
$distRoot = Assert-WorkspaceChild -Path $distRoot

$buildArguments = @(
    'build',
    $projectPath,
    '--configuration', 'Release',
    '--nologo',
    '-p:TreatWarningsAsErrors=true'
)

if ($NoRestore) {
    $buildArguments += '--no-restore'
}

if (![string]::IsNullOrWhiteSpace($SptPath)) {
    $resolvedSptPath = [System.IO.Path]::GetFullPath($SptPath)
    $assemblyPath = Join-Path $resolvedSptPath 'EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll'

    if (!(Test-Path -LiteralPath $assemblyPath -PathType Leaf)) {
        throw "SptPath does not point to a readable SPT installation: $resolvedSptPath"
    }

    $buildArguments += "-p:SptPath=$resolvedSptPath"
}

& dotnet @buildArguments
if ($LASTEXITCODE -ne 0) {
    throw "dotnet build failed with exit code $LASTEXITCODE."
}

if (!(Test-Path -LiteralPath $outputDll -PathType Leaf)) {
    throw "Build completed without the expected plugin DLL: $outputDll"
}

if (Test-Path -LiteralPath $packageRoot) {
    Remove-Item -LiteralPath $packageRoot -Recurse -Force
}

New-Item -ItemType Directory -Path $pluginRoot -Force | Out-Null
Copy-Item -LiteralPath $outputDll -Destination (Join-Path $pluginRoot 'SPT.Ready.dll')

New-Item -ItemType Directory -Path $distRoot -Force | Out-Null
if (Test-Path -LiteralPath $archivePath) {
    Remove-Item -LiteralPath $archivePath -Force
}

Compress-Archive -Path (Join-Path $packageRoot 'BepInEx') -DestinationPath $archivePath -CompressionLevel Optimal
Write-Output "Created $archivePath"
