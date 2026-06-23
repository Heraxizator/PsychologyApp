# Fix incorrect namespace replacements from FSD migration
$ErrorActionPreference = "Stop"
$repo = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$roots = @(
    (Join-Path $repo "PsychologyApp.Presentation"),
    (Join-Path $repo "PsychologyApp.Presentation.Tests")
)

$replacements = [ordered]@{
    "PsychologyApp.Presentation.Features.Practice.Model.Techniques" = "PsychologyApp.Presentation.Models.Practice.Techniques"
    "namespace PsychologyApp.Presentation.Services;" = "namespace PsychologyApp.Presentation.Shared.Navigation;"
    "namespace PsychologyApp.Presentation.Services" = "namespace PsychologyApp.Presentation.Shared.Navigation"
    'x:Class="PsychologyApp.Presentation.App"' = 'x:Class="PsychologyApp.Presentation.App.App"'
    'x:Class="PsychologyApp.Presentation.AppShell"' = 'x:Class="PsychologyApp.Presentation.App.AppShell"'
    'clr-namespace:PsychologyApp.Presentation"' = 'clr-namespace:PsychologyApp.Presentation.App"'
    "using AppShell = PsychologyApp.Presentation.App.AppShell;" = "using AppShell = PsychologyApp.Presentation.App.AppShell;"
    "PsychologyApp.Presentation.Shared.Navigation.Practice" = "PsychologyApp.Presentation.Features.Practice.Services"
    "PsychologyApp.Presentation.Shared.Navigation.Clean" = "PsychologyApp.Presentation.Features.Clean.Services"
    "PsychologyApp.Presentation.Shared.Navigation.Quotes" = "PsychologyApp.Presentation.Features.Motivator.Services"
    "PsychologyApp.Presentation.Shared.Navigation.Physics" = "PsychologyApp.Presentation.Features.Physics.Services"
    "PsychologyApp.Presentation.Shared.Navigation.Tests" = "PsychologyApp.Presentation.Features.Tests.Services"
    "PsychologyApp.Presentation.Shared.Navigation.Profile" = "PsychologyApp.Presentation.Features.Profile.Services"
    "PsychologyApp.Presentation.ViewModels" = "PsychologyApp.Presentation.Shared.Ui.ViewModels"
    "clr-namespace:PsychologyApp.Presentation.Controls" = "clr-namespace:PsychologyApp.Presentation.Shared.Controls"
    "namespace PsychologyApp.Presentation.Controls" = "namespace PsychologyApp.Presentation.Shared.Controls"
}

function Update-File($file) {
    $text = [IO.File]::ReadAllText($file)
    $original = $text
    foreach ($key in $replacements.Keys) {
        $text = $text.Replace($key, $replacements[$key])
    }
    if ($text -ne $original) {
        [IO.File]::WriteAllText($file, $text)
    }
}

foreach ($root in $roots) {
    if (-not (Test-Path $root)) { continue }
    Get-ChildItem $root -Recurse -Include *.cs,*.xaml -File | ForEach-Object { Update-File $_.FullName }
}

Write-Host "Fix script complete."
