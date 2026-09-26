param(
    [string]$OutputPath = "docs/collection-semantics-manifest.json",
    [string]$ExpectationPath = "docs/xedit-collection-expectations.json",
    [string]$ReportPath = "docs/collection-semantics-comparison.md",
    [switch]$Verify,
    [switch]$FailOnUnresolved
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$toolProject = Join-Path $repoRoot "tools/CollectionSemanticsAudit/CollectionSemanticsAudit.csproj"
$manifestPath = if ([System.IO.Path]::IsPathRooted($OutputPath)) {
    $OutputPath
} else {
    Join-Path $repoRoot $OutputPath
}
$expectationsPath = if ([System.IO.Path]::IsPathRooted($ExpectationPath)) {
    $ExpectationPath
} else {
    Join-Path $repoRoot $ExpectationPath
}
$comparisonPath = if ([System.IO.Path]::IsPathRooted($ReportPath)) {
    $ReportPath
} else {
    Join-Path $repoRoot $ReportPath
}
$arguments = @(
    "run",
    "--project", $toolProject,
    "--configuration", "Release",
    "--",
    $repoRoot,
    $manifestPath,
    $expectationsPath,
    $comparisonPath
)
if ($Verify) {
    $arguments += "--verify"
}
if ($FailOnUnresolved) {
    $arguments += "--fail-on-unresolved"
}

Push-Location $repoRoot
try {
    & dotnet @arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Collection semantics audit failed with exit code $LASTEXITCODE."
    }
}
finally {
    Pop-Location
}
