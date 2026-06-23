# Update namespaces and usings after FSD-lite migration
$ErrorActionPreference = "Stop"
$repo = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$paths = @(
    (Join-Path $repo "PsychologyApp.Presentation"),
    (Join-Path $repo "PsychologyApp.Presentation.Tests"),
    (Join-Path $repo "ARCHITECTURE.md")
)

$features = @("Review", "Onboarding", "Clean", "Physics", "Motivator", "Tests", "Profile", "Practice")

$replacements = [ordered]@{
    "namespace PsychologyApp.Presentation;" = "namespace PsychologyApp.Presentation.App;"
    "namespace PsychologyApp.Presentation.Common" = "namespace PsychologyApp.Presentation.Shared.Common"
    "namespace PsychologyApp.Presentation.Platform" = "namespace PsychologyApp.Presentation.Shared.Platform"
    "namespace PsychologyApp.Presentation.Abstractions" = "namespace PsychologyApp.Presentation.Shared.Abstractions"
    "namespace PsychologyApp.Presentation.DependencyInjection" = "namespace PsychologyApp.Presentation.Shared.DependencyInjection"
    "namespace PsychologyApp.Presentation.Controls" = "namespace PsychologyApp.Presentation.Shared.Controls"
    "using PsychologyApp.Presentation.Common" = "using PsychologyApp.Presentation.Shared.Common"
    "using PsychologyApp.Presentation.Platform" = "using PsychologyApp.Presentation.Shared.Platform"
    "using PsychologyApp.Presentation.Abstractions" = "using PsychologyApp.Presentation.Shared.Abstractions"
    "using PsychologyApp.Presentation.DependencyInjection" = "using PsychologyApp.Presentation.Shared.DependencyInjection"
    "using PsychologyApp.Presentation.Controls" = "using PsychologyApp.Presentation.Shared.Controls"
    "PsychologyApp.Presentation.Services.Dialogs" = "PsychologyApp.Presentation.Shared.Services.Dialogs"
    "PsychologyApp.Presentation.Services.Toasts" = "PsychologyApp.Presentation.Shared.Services.Toasts"
    "PsychologyApp.Presentation.Services.Shell" = "PsychologyApp.Presentation.Shared.Services.Shell"
    "PsychologyApp.Presentation.Services.Preferences" = "PsychologyApp.Presentation.Shared.Services.Preferences"
    "PsychologyApp.Presentation.Services.Factories" = "PsychologyApp.Presentation.Shared.Services.Factories"
    "using PsychologyApp.Presentation.Services.Dialogs" = "using PsychologyApp.Presentation.Shared.Services.Dialogs"
    "using PsychologyApp.Presentation.Services.Toasts" = "using PsychologyApp.Presentation.Shared.Services.Toasts"
    "using PsychologyApp.Presentation.Services.Shell" = "using PsychologyApp.Presentation.Shared.Services.Shell"
    "using PsychologyApp.Presentation.Services.Preferences" = "using PsychologyApp.Presentation.Shared.Services.Preferences"
    "using PsychologyApp.Presentation.Services.Factories" = "using PsychologyApp.Presentation.Shared.Services.Factories"
    "PsychologyApp.Presentation.Services.Navigation" = "PsychologyApp.Presentation.Shared.Navigation"
    "using PsychologyApp.Presentation.Services;" = "using PsychologyApp.Presentation.Shared.Navigation;"
    "using PsychologyApp.Presentation.Services" = "using PsychologyApp.Presentation.Shared.Navigation"
    "PsychologyApp.Presentation.UI." = "PsychologyApp.Presentation.Shared.UI."
    "using PsychologyApp.Presentation.UI." = "using PsychologyApp.Presentation.Shared.UI."
    'xmlns:components="clr-namespace:PsychologyApp.Presentation.UI.Components"' = 'xmlns:components="clr-namespace:PsychologyApp.Presentation.Shared.UI.Components"'
    "clr-namespace:PsychologyApp.Presentation.UI.Components" = "clr-namespace:PsychologyApp.Presentation.Shared.UI.Components"
    "clr-namespace:PsychologyApp.Presentation.UI.Converters" = "clr-namespace:PsychologyApp.Presentation.Shared.UI.Converters"
    "PsychologyApp.Presentation.Models.Common" = "PsychologyApp.Presentation.Shared.Model"
    "using PsychologyApp.Presentation.Models.Common" = "using PsychologyApp.Presentation.Shared.Model"
    "PsychologyApp.Presentation.ViewModels;" = "PsychologyApp.Presentation.Shared.Ui.ViewModels;"
    "using PsychologyApp.Presentation.ViewModels;" = "using PsychologyApp.Presentation.Shared.Ui.ViewModels;"
}

foreach ($f in $features) {
    $replacements["PsychologyApp.Presentation.ViewModels.$f"] = "PsychologyApp.Presentation.Features.$f.Ui.ViewModels"
    $replacements["using PsychologyApp.Presentation.ViewModels.$f"] = "using PsychologyApp.Presentation.Features.$f.Ui.ViewModels"
    $replacements["PsychologyApp.Presentation.Views.$f"] = "PsychologyApp.Presentation.Features.$f.Ui.Views"
    $replacements["using PsychologyApp.Presentation.Views.$f"] = "using PsychologyApp.Presentation.Features.$f.Ui.Views"
    $replacements["PsychologyApp.Presentation.Models.$f"] = "PsychologyApp.Presentation.Features.$f.Model"
    $replacements["using PsychologyApp.Presentation.Models.$f"] = "using PsychologyApp.Presentation.Features.$f.Model"
    $replacements["clr-namespace:PsychologyApp.Presentation.ViewModels.$f"] = "clr-namespace:PsychologyApp.Presentation.Features.$f.Ui.ViewModels"
    $replacements["clr-namespace:PsychologyApp.Presentation.Views.$f"] = "clr-namespace:PsychologyApp.Presentation.Features.$f.Ui.Views"
}

$serviceMap = @{
    Practice = "Practice"; Tests = "Tests"; Physics = "Physics"; Profile = "Profile"
    Clean = "Clean"; Motivator = "Quotes"
}
foreach ($entry in $serviceMap.GetEnumerator()) {
    $replacements["PsychologyApp.Presentation.Services.$($entry.Value)"] = "PsychologyApp.Presentation.Features.$($entry.Key).Services"
    $replacements["using PsychologyApp.Presentation.Services.$($entry.Value)"] = "using PsychologyApp.Presentation.Features.$($entry.Key).Services"
}

# Motivator quotes path already mapped via Quotes -> Motivator.Services

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

foreach ($base in $paths) {
    if (-not (Test-Path $base)) { continue }
    if (Test-Path $base -PathType Leaf) { Update-File $base; continue }
    Get-ChildItem $base -Recurse -Include *.cs,*.xaml,*.csproj,*.md -File | ForEach-Object { Update-File $_.FullName }
}

Write-Host "Namespace updates complete."
