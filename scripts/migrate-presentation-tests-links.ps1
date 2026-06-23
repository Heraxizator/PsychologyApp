# Rewrite Presentation.Tests Compile Link paths to canonical FSD locations
$ErrorActionPreference = "Stop"
$testsCsproj = Join-Path $PSScriptRoot "..\PsychologyApp.Presentation.Tests\PsychologyApp.Presentation.Tests.csproj"
$presRoot = Join-Path $PSScriptRoot "..\PsychologyApp.Presentation"
$content = Get-Content $testsCsproj -Raw

$pattern = '<Compile Include="\.\.\\PsychologyApp\.Presentation\\([^"]+)" Link="([^"]+)" />'
$matches = [regex]::Matches($content, $pattern)
$replacements = @{}

foreach ($m in $matches) {
    $oldRel = $m.Groups[1].Value -replace '\\', '/'
    $linkName = $m.Groups[2].Value
    if ($replacements.ContainsKey($oldRel)) { continue }

    $leaf = Split-Path $oldRel -Leaf
    $candidates = Get-ChildItem -Path $presRoot -Recurse -File -Filter $leaf |
        Where-Object { $_.FullName -notmatch '\\obj\\|\\bin\\' }

    if ($candidates.Count -eq 0) {
        Write-Warning "Missing: $oldRel"
        continue
    }

    $picked = $candidates | Sort-Object { $_.FullName.Length } | Select-Object -First 1
    $newRel = $picked.FullName.Substring($presRoot.Length + 1) -replace '\\', '/'
    $replacements[$oldRel] = $newRel
}

foreach ($k in $replacements.Keys) {
    $oldPattern = [regex]::Escape($k) -replace '/', '\\'
    $newPath = $replacements[$k] -replace '/', '\\'
    $content = $content -replace "\\\.\.\\PsychologyApp\.Presentation\\$oldPattern", "..\PsychologyApp.Presentation\$newPath"
}

Set-Content -Path $testsCsproj -Value $content -NoNewline
Write-Host "Updated $($replacements.Count) link paths."
