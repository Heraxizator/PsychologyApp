# Set namespace and x:Class from canonical FSD folder path
$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..\PsychologyApp.Presentation")).Path

function Get-SliceNamespace([string]$fullPath) {
    $rel = $fullPath.Substring($root.Length + 1)
    if ($rel -match '^(Pages|Widgets|Entities|Features)\\([^\\]+)\\') {
        return "PsychologyApp.Presentation.$($Matches[1]).$($Matches[2])"
    }
    return $null
}

Get-ChildItem -Path $root -Recurse -File -Filter *.cs | ForEach-Object {
    $ns = Get-SliceNamespace $_.FullName
    if (-not $ns) { return }
    $c = Get-Content $_.FullName -Raw
    $updated = [regex]::Replace($c, 'namespace PsychologyApp\.Presentation\.[A-Za-z0-9_.]+;', "namespace $ns;")
    if ($updated -ne $c) {
        Set-Content -Path $_.FullName -Value $updated -NoNewline
    }
}

Get-ChildItem -Path $root -Recurse -File -Include *.xaml,*.xaml.cs | ForEach-Object {
    $ns = Get-SliceNamespace $_.FullName
    if (-not $ns) { return }
    $c = Get-Content $_.FullName -Raw
    $className = [System.IO.Path]::GetFileNameWithoutExtension($_.Name)
    $c = [regex]::Replace($c, 'x:Class="PsychologyApp\.Presentation\.[^"]+"', "x:Class=`"$ns.$className`"")
    if ($_.Extension -eq '.xaml') {
        $c = [regex]::Replace($c, 'clr-namespace:PsychologyApp\.Presentation\.[^";]+', "clr-namespace:$ns")
    }
    Set-Content -Path $_.FullName -Value $c -NoNewline
}

Write-Host "Path-based namespaces applied."
