# Fix compile errors after canonical FSD bulk migration
$ErrorActionPreference = "Stop"
$pres = Join-Path $PSScriptRoot "..\PsychologyApp.Presentation"

function Update-AllFiles {
    param([string]$Root, [hashtable]$Replacements, [string[]]$Include = @("*.cs", "*.xaml"))
    foreach ($pattern in $Include) {
        Get-ChildItem -Path $Root -Recurse -File -Filter $pattern | ForEach-Object {
            $content = Get-Content $_.FullName -Raw
            $orig = $content
            foreach ($k in $Replacements.Keys) {
                $content = $content.Replace($k, $Replacements[$k])
            }
            if ($content -ne $orig) {
                Set-Content -Path $_.FullName -Value $content -NoNewline
            }
        }
    }
}

# Shared.Model -> Entities.FilterChip
Update-AllFiles $pres @{
    'using PsychologyApp.Presentation.Shared.Model;' = 'using PsychologyApp.Presentation.Entities.FilterChip;'
    'xmlns:common="clr-namespace:PsychologyApp.Presentation.Shared.Model"' = 'xmlns:common="clr-namespace:PsychologyApp.Presentation.Entities.FilterChip"'
}

# Technique body factory
Update-AllFiles (Join-Path $pres "Features\RunTechniqueSession\TechniqueBodyFactory.cs") @{
    'using PsychologyApp.Presentation.Widgets.TechniqueBodies.Bodies;' = 'using PsychologyApp.Presentation.Widgets.TechniqueBodies;'
}

# Theory page namespace in navigators only
foreach ($rel in @(
    "Shared\Navigation\IPracticeTheoryNavigator.cs",
    "Features\RunTechniqueSession\PracticeTheoryNavigator.cs"
)) {
    $path = Join-Path $pres $rel
    if (-not (Test-Path $path)) { continue }
    $c = Get-Content $path -Raw
    $c = $c.Replace('using PsychologyApp.Presentation.Pages.TechniqueSession;', 'using PsychologyApp.Presentation.Pages.TechniqueTheory;')
    Set-Content -Path $path -Value $c -NoNewline
}

# XAML xmlns: page namespaces incorrectly used for shared/widgets
Get-ChildItem -Path (Join-Path $pres "Pages") -Recurse -File -Filter *.xaml | ForEach-Object {
    $c = Get-Content $_.FullName -Raw
    $orig = $c
    $c = [regex]::Replace($c, 'xmlns:ui="clr-namespace:PsychologyApp\.Presentation\.Pages\.[^"]+"', 'xmlns:ui="clr-namespace:PsychologyApp.Presentation.Shared.UI.Components"')
    $c = [regex]::Replace($c, 'xmlns:behaviors="clr-namespace:PsychologyApp\.Presentation\.Pages\.[^"]+"', 'xmlns:behaviors="clr-namespace:PsychologyApp.Presentation.Shared.Common.Behaviors"')
    $c = [regex]::Replace($c, 'xmlns:motivatorUi="clr-namespace:PsychologyApp\.Presentation\.Pages\.[^"]+"', 'xmlns:motivatorUi="clr-namespace:PsychologyApp.Presentation.Widgets.QuoteCardView.xaml"')
    $c = [regex]::Replace($c, 'xmlns:quotes="clr-namespace:PsychologyApp\.Presentation\.Pages\.[^"]+"', 'xmlns:quotes="clr-namespace:PsychologyApp.Presentation.Entities.Quote"')
    $c = [regex]::Replace($c, 'xmlns:profileUi="clr-namespace:PsychologyApp\.Presentation\.Pages\.[^"]+"', 'xmlns:profileUi="clr-namespace:PsychologyApp.Presentation.Widgets.ProfileTechniqueCardView.xaml"')
    $c = [regex]::Replace($c, 'xmlns:practiceUi="clr-namespace:PsychologyApp\.Presentation\.Pages\.[^"]+"', 'xmlns:practiceUi="clr-namespace:PsychologyApp.Presentation.Widgets.TechniqueListCardView.xaml"')
    $c = [regex]::Replace($c, 'xmlns:technique="clr-namespace:PsychologyApp\.Presentation\.Pages\.[^"]+"', 'xmlns:technique="clr-namespace:PsychologyApp.Presentation.Models.Practice"')
    $c = [regex]::Replace($c, 'xmlns:controls="clr-namespace:PsychologyApp\.Presentation\.Pages\.TechniqueSession"', 'xmlns:controls="clr-namespace:PsychologyApp.Presentation.Widgets.TechniqueSessionShell"')
    $c = [regex]::Replace($c, 'xmlns:testsUi="clr-namespace:PsychologyApp\.Presentation\.Pages\.[^"]+"', 'xmlns:testsUi="clr-namespace:PsychologyApp.Presentation.Widgets.TestListCardView.xaml"')
    $c = [regex]::Replace($c, 'xmlns:physicsUi="clr-namespace:PsychologyApp\.Presentation\.Pages\.[^"]+"', 'xmlns:physicsUi="clr-namespace:PsychologyApp.Presentation.Widgets.PhysicsReasonCardView.xaml"')
    $c = [regex]::Replace($c, 'xmlns:musicUi="clr-namespace:PsychologyApp\.Presentation\.Pages\.[^"]+"', 'xmlns:musicUi="clr-namespace:PsychologyApp.Presentation.Widgets.MusicListCardView.xaml"')
    if ($c -ne $orig) { Set-Content -Path $_.FullName -Value $c -NoNewline }
}

# Namespace/type conflicts in Polarity and Question page slices
Get-ChildItem -Path (Join-Path $pres "Pages\Polarity") -Recurse -File -Filter *.cs | ForEach-Object {
    $c = Get-Content $_.FullName -Raw
    $orig = $c
    $c = $c -replace 'Command<Polarity>', 'Command<Models.Practice.Techniques.Polarity>'
    $c = $c -replace 'ObservableCollection<Polarity>', 'ObservableCollection<Models.Practice.Techniques.Polarity>'
    $c = $c -replace 'new Polarity\b', 'new Models.Practice.Techniques.Polarity'
    $c = $c -replace 'List<Polarity>', 'List<Models.Practice.Techniques.Polarity>'
    if ($c -ne $orig) { Set-Content -Path $_.FullName -Value $c -NoNewline }
}

Get-ChildItem -Path (Join-Path $pres "Pages\Question") -Recurse -File -Filter *.cs | ForEach-Object {
    $c = Get-Content $_.FullName -Raw
    $orig = $c
    $c = $c -replace 'List<Question>', 'List<Models.Tests.Question>'
    $c = $c -replace 'ObservableRangeCollection<Question>', 'ObservableRangeCollection<Models.Tests.Question>'
    if ($c -ne $orig) { Set-Content -Path $_.FullName -Value $c -NoNewline }
}

# Add missing usings to key files
$usingPatches = @{
    "Shared\Navigation\IPageFactory.cs" = @(
        'using PsychologyApp.Presentation.Pages.StartPhysics;',
        'using PsychologyApp.Presentation.Pages.TestHistory;',
        'using PsychologyApp.Presentation.Pages.ProfileOptions;',
        'using PsychologyApp.Presentation.Pages.ProfileInfo;',
        'using PsychologyApp.Presentation.Pages.ProfileDonate;',
        'using PsychologyApp.Presentation.Pages.ProfileSettings;',
        'using PsychologyApp.Presentation.Pages.TechniqueTheory;',
        'using PsychologyApp.Presentation.Pages.FindProblem;',
        'using PsychologyApp.Presentation.Pages.Question;',
        'using PsychologyApp.Presentation.Pages.StandardTest;',
        'using PsychologyApp.Presentation.Pages.AlternativeTest;',
        'using PsychologyApp.Presentation.Pages.TestResult;',
        'using PsychologyApp.Presentation.Pages.TechniqueCreated;'
    )
    "Shared\Navigation\MauiPageFactory.cs" = @(
        'using PsychologyApp.Presentation.Pages.StartPhysics;',
        'using PsychologyApp.Presentation.Pages.TestHistory;',
        'using PsychologyApp.Presentation.Pages.ProfileOptions;',
        'using PsychologyApp.Presentation.Pages.ProfileInfo;',
        'using PsychologyApp.Presentation.Pages.ProfileDonate;',
        'using PsychologyApp.Presentation.Pages.ProfileSettings;',
        'using PsychologyApp.Presentation.Pages.TechniqueTheory;',
        'using PsychologyApp.Presentation.Pages.FindProblem;',
        'using PsychologyApp.Presentation.Pages.Question;',
        'using PsychologyApp.Presentation.Pages.StandardTest;',
        'using PsychologyApp.Presentation.Pages.AlternativeTest;',
        'using PsychologyApp.Presentation.Pages.TestResult;',
        'using PsychologyApp.Presentation.Pages.TechniqueCreated;',
        'using PsychologyApp.Presentation.Features.RunTechniqueSession;'
    )
    "Features\ManageProfile\ProfilePageFactory.cs" = @(
        'using PsychologyApp.Presentation.Pages.ProfileOptions;',
        'using PsychologyApp.Presentation.Pages.ProfileInfo;',
        'using PsychologyApp.Presentation.Pages.ProfileDonate;',
        'using PsychologyApp.Presentation.Pages.ProfileSettings;'
    )
    "Features\RunTests\TestPageFactory.cs" = @(
        'using PsychologyApp.Presentation.Pages.TestHistory;',
        'using PsychologyApp.Presentation.Pages.FindProblem;',
        'using PsychologyApp.Presentation.Pages.Question;',
        'using PsychologyApp.Presentation.Pages.StandardTest;',
        'using PsychologyApp.Presentation.Pages.AlternativeTest;',
        'using PsychologyApp.Presentation.Pages.TestResult;'
    )
    "Features\RunTechniqueSession\TechniquePageFactory.cs" = @(
        'using PsychologyApp.Presentation.Pages.TechniqueCreated;'
    )
    "App\Providers\ProfileViewModelFactories.cs" = @(
        'using PsychologyApp.Presentation.Pages.ProfileOptions;',
        'using PsychologyApp.Presentation.Pages.ProfileInfo;',
        'using PsychologyApp.Presentation.Pages.ProfileDonate;',
        'using PsychologyApp.Presentation.Pages.ProfileSettings;'
    )
    "App\Providers\TestsViewModelFactories.cs" = @(
        'using PsychologyApp.Presentation.Pages.TestHistory;',
        'using PsychologyApp.Presentation.Pages.LuscherTest;',
        'using PsychologyApp.Presentation.Pages.TestResult;',
        'using PsychologyApp.Presentation.Pages.Question;',
        'using PsychologyApp.Presentation.Pages.FindProblem;'
    )
    "App\Providers\PhysicsViewModelFactories.cs" = @(
        'using PsychologyApp.Presentation.Pages.StartPhysics;'
    )
    "App\Providers\TechniqueSessionViewModelFactory.cs" = @(
        'using PsychologyApp.Presentation.Pages.TechniqueCreated;',
        'using PsychologyApp.Presentation.Pages.PaperList;',
        'using PsychologyApp.Presentation.Pages.Polarity;'
    )
    "App\Providers\PracticeFeatureServiceCollectionExtensions.cs" = @(
        'using PsychologyApp.Presentation.Pages.TechniqueTheory;'
    )
    "Pages\LuscherTest\LuscherTestViewModel.cs" = @(
        'using PsychologyApp.Presentation.Pages.BaseTest;'
    )
    "Pages\StandardTest\StandardTestPage.xaml.cs" = @(
        'using PsychologyApp.Presentation.Pages.LuscherTest;'
    )
    "Pages\AlternativeTest\AlternativeTestPage.xaml.cs" = @(
        'using PsychologyApp.Presentation.Pages.LuscherTest;'
    )
    "Features\SendReviewForm\FormViewModel.Send.cs" = @(
        'using PsychologyApp.Presentation.Pages.ReviewForm;'
    )
}

foreach ($rel in $usingPatches.Keys) {
    $path = Join-Path $pres $rel
    if (-not (Test-Path $path)) { continue }
    $c = Get-Content $path -Raw
    foreach ($u in $usingPatches[$rel]) {
        if ($c -notmatch [regex]::Escape($u)) {
            $c = $u + "`r`n" + $c
        }
    }
    Set-Content -Path $path -Value $c -NoNewline
}

Write-Host "Build fixes applied."
