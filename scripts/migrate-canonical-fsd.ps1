# Canonical FSD migration: Features/* -> Pages/, Widgets/, Entities/, Features/ (slices)
$ErrorActionPreference = "Stop"
$root = Join-Path $PSScriptRoot "..\PsychologyApp.Presentation"
Set-Location $root

function Move-Slice {
    param([string]$From, [string]$To)
    if (-not (Test-Path $From)) { return }
    $toDir = Split-Path $To -Parent
    if ($toDir -and -not (Test-Path $toDir)) {
        New-Item -ItemType Directory -Path $toDir -Force | Out-Null
    }
    if (Test-Path $To) { Remove-Item $To -Force -Recurse -ErrorAction SilentlyContinue }
    git mv $From $To 2>$null
    if (-not (Test-Path $To)) {
        Move-Item $From $To -Force
    }
}

# --- Review pilot ---
Move-Slice "Features\Review\Ui\Views\FormPage.xaml" "Pages\ReviewForm\FormPage.xaml"
Move-Slice "Features\Review\Ui\Views\FormPage.xaml.cs" "Pages\ReviewForm\FormPage.xaml.cs"
Move-Slice "Features\Review\Ui\ViewModels\FormViewModel.cs" "Pages\ReviewForm\FormViewModel.cs"
Move-Slice "Features\Review\Ui\ViewModels\FormViewModel.Labels.cs" "Pages\ReviewForm\FormViewModel.Labels.cs"
Move-Slice "Features\Review\Ui\ViewModels\FormViewModel.Send.cs" "Features\SendReviewForm\FormViewModel.Send.cs"
Move-Slice "Features\Review\Ui\ViewModels\FormViewModel.Send.Channels.cs" "Features\SendReviewForm\FormViewModel.Send.Channels.cs"
Move-Slice "Features\Review\Services\ReviewPageFactory.cs" "Pages\ReviewForm\ReviewFormPageFactory.cs"

# --- Onboarding ---
Get-ChildItem "Features\Onboarding" -Recurse -File | ForEach-Object {
    $rel = $_.FullName.Substring((Resolve-Path "Features\Onboarding").Path.Length + 1)
    if ($rel -match '\\Views\\') {
        Move-Slice $_.FullName "Pages\Onboarding\$($_.Name)"
    } elseif ($rel -match '\\ViewModels\\') {
        Move-Slice $_.FullName "Pages\Onboarding\$($_.Name)"
    } else {
        Move-Slice $_.FullName "Features\CompleteOnboarding\$($_.Name)"
    }
}

# --- Clean ---
Get-ChildItem "Features\Clean\Ui\Views" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-Slice $_.FullName "Pages\MusicPlayer\$($_.Name)"
}
Get-ChildItem "Features\Clean\Ui\ViewModels" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-Slice $_.FullName "Pages\MusicPlayer\$($_.Name)"
}
Get-ChildItem "Features\Clean\Ui\Components" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-Slice $_.FullName "Widgets\MusicListCard\$($_.Name)"
}
Get-ChildItem "Features\Clean\Model" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-Slice $_.FullName "Entities\Audio\$($_.Name)"
}
Get-ChildItem "Features\Clean\Services" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-Slice $_.FullName "Features\PlayMusic\$($_.Name)"
}

# --- Motivator ---
Get-ChildItem "Features\Motivator\Ui\Views" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-Slice $_.FullName "Pages\QuoteFeed\$($_.Name)"
}
Get-ChildItem "Features\Motivator\Ui\ViewModels" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-Slice $_.FullName "Pages\QuoteFeed\$($_.Name)"
}
Get-ChildItem "Features\Motivator\Ui\Components" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-Slice $_.FullName "Widgets\QuoteCard\$($_.Name)"
}
Get-ChildItem "Features\Motivator\Model" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-Slice $_.FullName "Entities\Quote\$($_.Name)"
}
if (Test-Path "Shared\Model\Quotes\QuoteItem.cs") {
    Move-Slice "Shared\Model\Quotes\QuoteItem.cs" "Entities\Quote\QuoteItem.cs"
}
Get-ChildItem "Features\Motivator\Services" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-Slice $_.FullName "Features\ManageQuotes\$($_.Name)"
}

# --- Physics ---
Get-ChildItem "Features\Physics\Ui\Views" -File -ErrorAction SilentlyContinue | ForEach-Object {
    $dest = if ($_.Name -like "Start*") { "Pages\StartPhysics" } else { "Pages\PhysicsSearch" }
    Move-Slice $_.FullName "$dest\$($_.Name)"
}
Get-ChildItem "Features\Physics\Ui\ViewModels" -File -ErrorAction SilentlyContinue | ForEach-Object {
    $dest = if ($_.Name -like "Start*") { "Pages\StartPhysics" } else { "Pages\PhysicsSearch" }
    Move-Slice $_.FullName "$dest\$($_.Name)"
}
Get-ChildItem "Features\Physics\Ui\Components" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-Slice $_.FullName "Widgets\PhysicsReasonCard\$($_.Name)"
}
Get-ChildItem "Features\Physics\Model" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-Slice $_.FullName "Entities\Physics\$($_.Name)"
}
Get-ChildItem "Features\Physics\Services" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-Slice $_.FullName "Features\SearchPhysics\$($_.Name)"
}

Write-Host "Partial migration moves completed. Run update-canonical-fsd-namespaces.ps1 next."
