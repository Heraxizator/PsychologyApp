# Consolidate widget code-behind into proper slice folders (fix *.xaml/ orphan folders)
$ErrorActionPreference = "Stop"
$pres = Join-Path $PSScriptRoot "..\PsychologyApp.Presentation"
$widgets = Join-Path $pres "Widgets"

Get-ChildItem -Path $widgets -Directory -Filter "*View.xaml" | ForEach-Object {
    $orphanDir = $_.FullName
    $viewName = $_.Name -replace '\.xaml$',''          # QuoteCardView
    $sliceName = $viewName -replace 'View$',''        # QuoteCard
    $targetDir = Join-Path $widgets $sliceName

    if (-not (Test-Path $targetDir)) {
        New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
    }

    Get-ChildItem -Path $orphanDir -Filter *.cs | ForEach-Object {
        $dest = Join-Path $targetDir $_.Name
        if (Test-Path $dest) { Remove-Item $dest -Force }
        Move-Item $_.FullName $dest -Force
        $content = Get-Content $dest -Raw
        $ns = "PsychologyApp.Presentation.Widgets.$sliceName"
        $content = [regex]::Replace($content, 'namespace PsychologyApp\.Presentation\.Widgets\.[^;]+;', "namespace $ns;")
        Set-Content -Path $dest -Value $content -NoNewline
    }

    if ((Get-ChildItem $orphanDir -Force | Measure-Object).Count -eq 0) {
        Remove-Item $orphanDir -Force -Recurse
    }
}

# Luscher color grid widget under Pages
$gridOrphan = Join-Path $pres "Pages\LuscherColorGridView.xaml"
$gridTarget = Join-Path $pres "Pages\LuscherColorGridView"
if (Test-Path $gridOrphan) {
    if (-not (Test-Path $gridTarget)) { New-Item -ItemType Directory -Path $gridTarget -Force | Out-Null }
    Get-ChildItem $gridOrphan -Filter *.cs | ForEach-Object {
        $dest = Join-Path $gridTarget $_.Name
        if (Test-Path $dest) { Remove-Item $dest -Force }
        Move-Item $_.FullName $dest -Force
        $content = Get-Content $dest -Raw
        $content = [regex]::Replace($content, 'namespace PsychologyApp\.Presentation\.Pages\.[^;]+;', 'namespace PsychologyApp.Presentation.Pages.LuscherColorGridView;')
        Set-Content -Path $dest -Value $content -NoNewline
    }
    if ((Get-ChildItem $gridOrphan -Force | Measure-Object).Count -eq 0) {
        Remove-Item $gridOrphan -Force -Recurse
    }
}

# Fix xmlns references to old widget namespaces
$nsMap = @{
    'PsychologyApp.Presentation.Widgets.QuoteCardView.xaml' = 'PsychologyApp.Presentation.Widgets.QuoteCard'
    'PsychologyApp.Presentation.Widgets.TechniqueListCardView.xaml' = 'PsychologyApp.Presentation.Widgets.TechniqueListCard'
    'PsychologyApp.Presentation.Widgets.ProfileTechniqueCardView.xaml' = 'PsychologyApp.Presentation.Widgets.ProfileTechniqueCard'
    'PsychologyApp.Presentation.Widgets.TestListCardView.xaml' = 'PsychologyApp.Presentation.Widgets.TestListCard'
    'PsychologyApp.Presentation.Widgets.MusicListCardView.xaml' = 'PsychologyApp.Presentation.Widgets.MusicListCard'
    'PsychologyApp.Presentation.Widgets.PhysicsReasonCardView.xaml' = 'PsychologyApp.Presentation.Widgets.PhysicsReasonCard'
    'PsychologyApp.Presentation.Widgets.MoodStripView.xaml' = 'PsychologyApp.Presentation.Widgets.MoodStrip'
    'PsychologyApp.Presentation.Widgets.TodayPracticeRowView.xaml' = 'PsychologyApp.Presentation.Widgets.TodayPracticeRow'
}

Get-ChildItem -Path $pres -Recurse -File -Include *.cs,*.xaml | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $orig = $content
    foreach ($k in $nsMap.Keys) {
        $content = $content.Replace($k, $nsMap[$k])
    }
    if ($content -ne $orig) { Set-Content -Path $_.FullName -Value $content -NoNewline }
}

Write-Host "Widget folders consolidated."
