# Fix XAML xmlns after canonical FSD migration
$ErrorActionPreference = "Stop"
$pres = Join-Path $PSScriptRoot "..\PsychologyApp.Presentation"

$sharedUi = 'clr-namespace:PsychologyApp.Presentation.Shared.UI.Components'
$sharedBehaviors = 'clr-namespace:PsychologyApp.Presentation.Shared.Common.Behaviors'
$sharedConverters = 'clr-namespace:PsychologyApp.Presentation.Shared.UI.Converters'

Get-ChildItem -Path $pres -Recurse -File -Filter *.xaml | ForEach-Object {
    $path = $_.FullName
    $c = Get-Content $path -Raw
    $orig = $c

    # Shared UI inside technique bodies and widgets
    $c = [regex]::Replace($c, 'xmlns:ui="clr-namespace:PsychologyApp\.Presentation\.Widgets\.TechniqueBodies"', "xmlns:ui=`"$sharedUi`"")
    $c = [regex]::Replace($c, 'xmlns:ui="clr-namespace:PsychologyApp\.Presentation\.Widgets\.TechniqueSessionShell"', "xmlns:ui=`"$sharedUi`"")
    $c = [regex]::Replace($c, 'xmlns:ui="clr-namespace:PsychologyApp\.Presentation\.Widgets\.(MusicListCard|PhysicsReasonCard|ProfileTechniqueCard|TechniqueListCard|TodayPracticeRow)"', "xmlns:ui=`"$sharedUi`"")

    # Models.Practice -> Models.Practice.Techniques for TechniqueGroup/TheorySection
    $c = $c.Replace('xmlns:technique="clr-namespace:PsychologyApp.Presentation.Models.Practice"', 'xmlns:technique="clr-namespace:PsychologyApp.Presentation.Models.Practice.Techniques"')

    # Question page models
    $c = $c.Replace('xmlns:models="clr-namespace:PsychologyApp.Presentation.Pages.Question;assembly=PsychologyApp.Presentation.Core"', 'xmlns:models="clr-namespace:PsychologyApp.Presentation.Models.Tests;assembly=PsychologyApp.Presentation.Core"')

    # Music player entity + widget prefixes
    if ($path -like '*\MusicPlayer\MusicPlayerPage.xaml') {
        $c = $c.Replace('xmlns:clean="clr-namespace:PsychologyApp.Presentation.Pages.MusicPlayer"', 'xmlns:clean="clr-namespace:PsychologyApp.Presentation.Entities.Audio"')
        $c = $c.Replace('xmlns:cleanUi="clr-namespace:PsychologyApp.Presentation.Pages.MusicPlayer"', 'xmlns:cleanUi="clr-namespace:PsychologyApp.Presentation.Widgets.MusicListCard"')
    }

    # Entity data templates on pages
    if ($path -like '*\FindProblem\FindProblemPage.xaml') {
        $c = $c -replace 'xmlns:tests="clr-namespace:PsychologyApp\.Presentation\.Pages\.FindProblem"', 'xmlns:tests="clr-namespace:PsychologyApp.Presentation.Entities.Test"'
    }
    if ($path -like '*\PhysicsSearch\PhysicsSearchPage.xaml') {
        if ($c -notmatch 'xmlns:physics=') {
            $c = $c -replace '(xmlns:vm="[^"]+")', "`$1`r`n             xmlns:physics=`"clr-namespace:PsychologyApp.Presentation.Entities.Physics`""
        }
        $c = $c -replace 'x:DataType="vm:PhysicsReasonItem"', 'x:DataType="physics:PhysicsReasonItem"'
    }
    if ($path -like '*\ProfileSettings\SettingsPage.xaml') {
        $c = $c -replace 'xmlns:profile="clr-namespace:PsychologyApp\.Presentation\.Pages\.ProfileSettings"', "xmlns:profile=`"$sharedConverters`""
    }
    if ($path -like '*\ProfileUser\UserPage.xaml') {
        $c = $c -replace 'xmlns:profile="clr-namespace:PsychologyApp\.Presentation\.Pages\.ProfileUser"', 'xmlns:profile="clr-namespace:PsychologyApp.Presentation.Entities.Profile"'
    }
    if ($path -like '*\TestHistory\TestHistoryPage.xaml') {
        $c = $c -replace 'xmlns:tests="clr-namespace:PsychologyApp\.Presentation\.Pages\.TestHistory"', 'xmlns:tests="clr-namespace:PsychologyApp.Presentation.Entities.Test"'
    }
    if ($path -like '*\TestsList\TestsListPage.xaml') {
        $c = $c -replace 'xmlns:tests="clr-namespace:PsychologyApp\.Presentation\.Pages\.TestsList"', 'xmlns:tests="clr-namespace:PsychologyApp.Presentation.Entities.Test"'
    }

    # Luscher color grid lives in its own page slice
    if ($path -like '*\StandardTest\StandardTestPage.xaml' -or $path -like '*\AlternativeTest\AlternativeTestPage.xaml') {
        if ($c -notmatch 'xmlns:grid=') {
            $c = $c -replace '(xmlns:tests="clr-namespace:PsychologyApp\.Presentation\.Pages\.LuscherTest")', "`$1`r`n             xmlns:grid=`"clr-namespace:PsychologyApp.Presentation.Pages.LuscherColorGridView`""
        }
        $c = $c.Replace('<tests:LuscherColorGridView', '<grid:LuscherColorGridView')
    }

    # Today practice row widget
    if ($path -like '*\Techniques\TechniquesPage.xaml') {
        if ($c -notmatch 'xmlns:today=') {
            $c = $c -replace '(xmlns:practiceUi="clr-namespace:PsychologyApp\.Presentation\.Widgets\.TechniqueListCard")', "`$1`r`n             xmlns:today=`"clr-namespace:PsychologyApp.Presentation.Widgets.TodayPracticeRow`""
        }
        $c = $c.Replace('<practiceUi:TodayPracticeRowView', '<today:TodayPracticeRowView')
    }

    if ($c -ne $orig) {
        Set-Content -Path $path -Value $c -NoNewline
    }
}

Write-Host "XAML xmlns fixed."
