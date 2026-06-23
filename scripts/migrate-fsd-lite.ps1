# Complete FSD-lite migration (idempotent)
$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..\PsychologyApp.Presentation")).Path
Set-Location $root

function Ensure-Dir($path) {
    if (-not (Test-Path $path)) { New-Item -ItemType Directory -Path $path -Force | Out-Null }
}

function Move-ItemSafe($from, $to) {
    if (-not (Test-Path $from)) { return }
    if ((Get-Item $from).PSIsContainer -and ((Get-ChildItem $from -Force | Measure-Object).Count -eq 0)) {
        Remove-Item $from -Force -Recurse -ErrorAction SilentlyContinue
        return
    }
    Ensure-Dir (Split-Path $to -Parent)
    if (Test-Path $to) { return }
    git mv $from $to 2>$null
    if ($LASTEXITCODE -ne 0) { Move-Item -LiteralPath $from -Destination $to -Force }
}

function Merge-Dir($from, $to) {
    if (-not (Test-Path $from)) { return }
    Ensure-Dir $to
    Get-ChildItem $from -Force | ForEach-Object {
        $dest = Join-Path $to $_.Name
        if (Test-Path $dest) {
            if ($_.PSIsContainer) { Merge-Dir $_.FullName $dest }
        } else {
            Move-ItemSafe $_.FullName $dest
        }
    }
    if ((Test-Path $from) -and ((Get-ChildItem $from -Force | Measure-Object).Count -eq 0)) {
        Remove-Item $from -Recurse -Force -ErrorAction SilentlyContinue
    }
}

$features = @("Review", "Onboarding", "Clean", "Physics", "Motivator", "Tests", "Profile", "Practice")
foreach ($f in $features) {
    @("Ui/ViewModels", "Ui/Views", "Ui/Components", "Model", "Services") | ForEach-Object {
        Ensure-Dir (Join-Path $root "Features/$f/$_")
    }
}

@("App", "Shared/Common", "Shared/Platform", "Shared/Abstractions", "Shared/DependencyInjection",
  "Shared/Navigation", "Shared/Services/Dialogs", "Shared/Services/Toasts", "Shared/Services/Shell",
  "Shared/Services/Preferences", "Shared/Controls", "Shared/UI", "Shared/Model", "Shared/Ui/ViewModels",
  "Shared/Services/Factories") | ForEach-Object { Ensure-Dir (Join-Path $root $_) }

# App
@("App.xaml", "App.xaml.cs", "AppShell.xaml", "AppShell.xaml.cs", "MauiProgram.cs") | ForEach-Object {
    Move-ItemSafe (Join-Path $root $_) (Join-Path $root "App/$_")
}

# Shared top-level merges
Merge-Dir (Join-Path $root "Common") (Join-Path $root "Shared/Common")
Merge-Dir (Join-Path $root "Platform") (Join-Path $root "Shared/Platform")
Merge-Dir (Join-Path $root "Abstractions") (Join-Path $root "Shared/Abstractions")
Merge-Dir (Join-Path $root "DependencyInjection") (Join-Path $root "Shared/DependencyInjection")
Merge-Dir (Join-Path $root "Controls") (Join-Path $root "Shared/Controls")

@("MauiPageFactory.cs", "IPageFactory.cs", "NavigationService.cs", "NavigationContext.cs", "NavigationCoordinator.cs", "PageViewModelActivator.cs") | ForEach-Object {
    Move-ItemSafe (Join-Path $root "Services/$_") (Join-Path $root "Shared/Navigation/$_")
}
Merge-Dir (Join-Path $root "Services/Dialogs") (Join-Path $root "Shared/Services/Dialogs")
Merge-Dir (Join-Path $root "Services/Toasts") (Join-Path $root "Shared/Services/Toasts")
Merge-Dir (Join-Path $root "Services/Shell") (Join-Path $root "Shared/Services/Shell")
Merge-Dir (Join-Path $root "Services/Preferences") (Join-Path $root "Shared/Services/Preferences")
Merge-Dir (Join-Path $root "Services/Factories") (Join-Path $root "Shared/Services/Factories")

Get-ChildItem (Join-Path $root "ViewModels") -Filter "BaseViewModel*" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-ItemSafe $_.FullName (Join-Path $root "Shared/Ui/ViewModels/$($_.Name)")
}
Merge-Dir (Join-Path $root "Models/Common") (Join-Path $root "Shared/Model")

$featureServiceMap = @{
    Review = @(); Onboarding = @(); Clean = @("Clean"); Physics = @("Physics")
    Motivator = @("Quotes"); Tests = @("Tests"); Profile = @("Profile"); Practice = @("Practice")
}
foreach ($f in $features) {
    $featRoot = Join-Path $root "Features/$f"
    Merge-Dir (Join-Path $root "ViewModels/$f") (Join-Path $featRoot "Ui/ViewModels")
    Merge-Dir (Join-Path $root "Views/$f") (Join-Path $featRoot "Ui/Views")
    Merge-Dir (Join-Path $root "Models/$f") (Join-Path $featRoot "Model")
    foreach ($svc in $featureServiceMap[$f]) {
        Merge-Dir (Join-Path $root "Services/$svc") (Join-Path $featRoot "Services")
    }
}

@{
    "TestPageFactory.cs" = "Tests"; "TechniquePageFactory.cs" = "Practice"; "ProfilePageFactory.cs" = "Profile"
}.GetEnumerator() | ForEach-Object {
    Move-ItemSafe (Join-Path $root "Services/$($_.Key)") (Join-Path $root "Features/$($_.Value)/Services/$($_.Key)")
}

Merge-Dir (Join-Path $root "UI") (Join-Path $root "Shared/UI")

$uiComponentMap = @{
    "TestListCardView" = "Tests"; "AlgorithmBoxView" = "Tests"; "QuoteCardView" = "Motivator"
    "MusicListCardView" = "Clean"; "PhysicsReasonCardView" = "Physics"; "ProfileTechniqueCardView" = "Profile"
    "TodayPracticeRowView" = "Practice"; "MoodStripView" = "Practice"; "TechniqueListCardView" = "Practice"
}
$compDir = Join-Path $root "Shared/UI/Components"
if (Test-Path $compDir) {
    foreach ($entry in $uiComponentMap.GetEnumerator()) {
        Get-ChildItem $compDir -Filter "$($entry.Key).*" -ErrorAction SilentlyContinue | ForEach-Object {
            Move-ItemSafe $_.FullName (Join-Path $root "Features/$($entry.Value)/Ui/Components/$($_.Name)")
        }
    }
}
if (Test-Path (Join-Path $root "Shared/UI/Techniques")) {
    Merge-Dir (Join-Path $root "Shared/UI/Techniques") (Join-Path $root "Features/Practice/Ui/Techniques")
}

# Remove empty legacy dirs
@("Common", "Platform", "Abstractions", "DependencyInjection", "Controls", "UI",
  "ViewModels", "Views", "Models", "Services") | ForEach-Object {
    $p = Join-Path $root $_
    if ((Test-Path $p) -and ((Get-ChildItem $p -Recurse -Force -ErrorAction SilentlyContinue | Measure-Object).Count -eq 0)) {
        Remove-Item $p -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Host "Migration file moves complete."
