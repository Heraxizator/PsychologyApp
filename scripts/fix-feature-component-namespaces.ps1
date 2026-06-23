# Fix feature-specific component and body namespaces/x:Class
$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..\PsychologyApp.Presentation")).Path

$featureComponents = @{
    "Practice" = @("TodayPracticeRowView", "MoodStripView", "TechniqueListCardView")
    "Tests" = @("TestListCardView", "AlgorithmBoxView")
    "Motivator" = @("QuoteCardView")
    "Clean" = @("MusicListCardView")
    "Physics" = @("PhysicsReasonCardView")
    "Profile" = @("ProfileTechniqueCardView")
}

function Update-File($file, $replacements) {
    $text = [IO.File]::ReadAllText($file)
    $original = $text
    foreach ($entry in $replacements.GetEnumerator()) {
        $text = $text.Replace($entry.Key, $entry.Value)
    }
    if ($text -ne $original) {
        [IO.File]::WriteAllText($file, $text)
    }
}

foreach ($entry in $featureComponents.GetEnumerator()) {
    $feat = $entry.Key
    $ns = "PsychologyApp.Presentation.Features.$feat.Ui.Components"
    foreach ($name in $entry.Value) {
        $dir = Join-Path $root "Features/$feat/Ui/Components"
        $cs = Join-Path $dir "$name.xaml.cs"
        $xaml = Join-Path $dir "$name.xaml"
        if (Test-Path $cs) {
            Update-File $cs @{
                "namespace PsychologyApp.Presentation.Shared.UI.Components;" = "namespace $ns;"
            }
        }
        if (Test-Path $xaml) {
            Update-File $xaml @{
                "x:Class=`"PsychologyApp.Presentation.Shared.UI.Components.$name`"" = "x:Class=`"$ns.$name`""
            }
        }
    }
}

$bodyNs = "PsychologyApp.Presentation.Features.Practice.Ui.Techniques.Bodies"
$bodyDir = Join-Path $root "Features/Practice/Ui/Techniques/Bodies"
if (Test-Path $bodyDir) {
    Get-ChildItem $bodyDir -Filter "*.cs" | ForEach-Object {
        Update-File $_.FullName @{
            "namespace PsychologyApp.Presentation.Shared.UI.Techniques.Bodies;" = "namespace $bodyNs;"
        }
    }
    Get-ChildItem $bodyDir -Filter "*.xaml" | ForEach-Object {
        $name = [IO.Path]::GetFileNameWithoutExtension($_.Name)
        Update-File $_.FullName @{
            "x:Class=`"PsychologyApp.Presentation.Shared.UI.Techniques.Bodies.$name`"" = "x:Class=`"$bodyNs.$name`""
        }
    }
}

Write-Host "Feature component namespaces fixed."
