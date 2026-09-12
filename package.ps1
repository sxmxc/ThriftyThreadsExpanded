[CmdletBinding()]
param(
    [ValidateSet('Mono', 'Il2cpp')]
    [string]$Configuration = 'Il2cpp',
    [string]$DllPath,
    [switch]$Build
)

$ErrorActionPreference = 'Stop'

$projectRoot = $PSScriptRoot
$projectFile = Join-Path $projectRoot 'ThriftyThreadsExpanded.csproj'
$outputDirectory = Join-Path $projectRoot "bin\$Configuration"
$dllName = "ThriftyThreadsExpanded_$Configuration.dll"
$packagesDirectory = Join-Path $projectRoot 'packages'
$archivePath = Join-Path $packagesDirectory 'ThriftyThreadsExpanded.zip'

if ($Build) {
    Write-Host "Building $Configuration..."
    & dotnet build $projectFile -c $Configuration -p:AutomateLocalDeployment=false
    if ($LASTEXITCODE -ne 0) {
        throw "The $Configuration build failed with exit code $LASTEXITCODE."
    }
}

if ($DllPath) {
    $dllPath = (Resolve-Path -LiteralPath $DllPath -ErrorAction Stop).Path
    $dllCandidates = @([System.IO.FileInfo]::new($dllPath))
} else {
    $dllCandidates = @(Get-ChildItem -LiteralPath $outputDirectory -Filter $dllName -File -Recurse -ErrorAction SilentlyContinue)
}
if ($dllCandidates.Count -ne 1) {
    throw "Expected exactly one DLL to package. Build the mod first, pass -Build, or provide -DllPath."
}

$dllPath = $dllCandidates[0].FullName

New-Item -ItemType Directory -Path $packagesDirectory -Force | Out-Null
$stagingDirectory = Join-Path ([System.IO.Path]::GetTempPath()) ("ThriftyThreadsExpanded-package-" + [guid]::NewGuid().ToString('N'))
$modsDirectory = Join-Path $stagingDirectory 'mods'

try {
    New-Item -ItemType Directory -Path $modsDirectory -Force | Out-Null
    Copy-Item -LiteralPath $dllPath -Destination (Join-Path $modsDirectory $dllName)
    Compress-Archive -Path $modsDirectory -DestinationPath $archivePath -Force
} finally {
    if (Test-Path -LiteralPath $stagingDirectory) {
        Remove-Item -LiteralPath $stagingDirectory -Recurse -Force
    }
}

Write-Host "Created $archivePath"
