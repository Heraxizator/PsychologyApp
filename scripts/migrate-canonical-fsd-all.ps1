# Canonical FSD: bulk file moves from legacy Features/* layout
$ErrorActionPreference = "Stop"
$pres = Join-Path $PSScriptRoot "..\PsychologyApp.Presentation"
Set-Location $pres

function Ensure-Dir([string]$Path) {
    if (-not (Test-Path $Path)) { New-Item -ItemType Directory -Path $Path -Force | Out-Null }
}

function Move-File([string]$From, [string]$To) {
    if (-not (Test-Path $From)) { return }
    Ensure-Dir (Split-Path $To -Parent)
    if (Test-Path $To) { Remove-Item $To -Force }
    Move-Item -Path $From -Destination $To -Force
}

function PageFolder([string]$pageName) {
    switch -Regex ($pageName) {
        '^FormPage' { return 'ReviewForm' }
        '^OnboardingPage' { return 'Onboarding' }
        '^MusicPlayerPage' { return 'MusicPlayer' }
        '^QuotePage' { return 'QuoteFeed' }
        '^PhysicsSearchPage' { return 'PhysicsSearch' }
        '^StartPhysicsPage' { return 'StartPhysics' }
        '^TestsListPage' { return 'TestsList' }
        '^TestHistoryPage' { return 'TestHistory' }
        '^FindProblemPage' { return 'FindProblem' }
        '^QuestionPage' { return 'Question' }
        '^StandardTestPage' { return 'StandardTest' }
        '^AlternativeTestPage' { return 'AlternativeTest' }
        '^TestResultPage' { return 'TestResult' }
        '^LuscherTestPage' { return 'LuscherTest' }
        '^UserPage' { return 'ProfileUser' }
        '^SettingsPage' { return 'ProfileSettings' }
        '^OptionsPage' { return 'ProfileOptions' }
        '^InfoPage' { return 'ProfileInfo' }
        '^DonatePage' { return 'ProfileDonate' }
        '^TechniquesPage' { return 'Techniques' }
        '^TechniqueSessionPage' { return 'TechniqueSession' }
        '^TheoryPage' { return 'TechniqueTheory' }
        '^DesignerPage' { return 'TechniqueDesigner' }
        '^CreatedPage' { return 'TechniqueCreated' }
        default { return ($pageName -replace 'Page$', '') }
    }
}

function WidgetFolder([string]$componentName) {
    return ($componentName -replace 'View$', '')
}

function Move-LegacyFeature([string]$legacyName, [string]$entityFolder, [string]$featureFolder) {
    $base = Join-Path "Features" $legacyName
    if (-not (Test-Path $base)) { return }

    Get-ChildItem (Join-Path $base "Ui\Views") -File -ErrorAction SilentlyContinue | ForEach-Object {
        $folder = PageFolder ($_.BaseName)
        Move-File $_.FullName "Pages\$folder\$($_.Name)"
    }
    Get-ChildItem (Join-Path $base "Ui\ViewModels") -File -Recurse -ErrorAction SilentlyContinue | ForEach-Object {
        $pageKey = $_.Name -replace '\..*$','' -replace 'ViewModel$','Page'
        if ($pageKey -notlike '*Page') { $pageKey = "${pageKey}Page" }
        $folder = PageFolder $pageKey
        if ($folder -eq 'Page') { $folder = PageFolder ($legacyName + 'Page') }
        Move-File $_.FullName "Pages\$folder\$($_.Name)"
    }
    Get-ChildItem (Join-Path $base "Ui\Components") -File -ErrorAction SilentlyContinue | ForEach-Object {
        $folder = WidgetFolder $_.BaseName
        Move-File $_.FullName "Widgets\$folder\$($_.Name)"
    }
    Get-ChildItem (Join-Path $base "Model") -File -ErrorAction SilentlyContinue | ForEach-Object {
        Move-File $_.FullName "Entities\$entityFolder\$($_.Name)"
    }
    Get-ChildItem (Join-Path $base "Services") -File -ErrorAction SilentlyContinue | ForEach-Object {
        Move-File $_.FullName "Features\$featureFolder\$($_.Name)"
    }
    Get-ChildItem (Join-Path $base "DependencyInjection") -File -ErrorAction SilentlyContinue | ForEach-Object {
        Move-File $_.FullName "App\Providers\Legacy$legacyName$($_.Name)"
    }
}

# Review (manual split for send feature)
Move-File "Features\Review\Ui\Views\FormPage.xaml" "Pages\ReviewForm\FormPage.xaml"
Move-File "Features\Review\Ui\Views\FormPage.xaml.cs" "Pages\ReviewForm\FormPage.xaml.cs"
Move-File "Features\Review\Ui\ViewModels\FormViewModel.cs" "Pages\ReviewForm\FormViewModel.cs"
Move-File "Features\Review\Ui\ViewModels\FormViewModel.Labels.cs" "Pages\ReviewForm\FormViewModel.Labels.cs"
Move-File "Features\Review\Ui\ViewModels\FormViewModel.Send.cs" "Features\SendReviewForm\FormViewModel.Send.cs"
Move-File "Features\Review\Ui\ViewModels\FormViewModel.Send.Channels.cs" "Features\SendReviewForm\FormViewModel.Send.Channels.cs"
Move-File "Features\Review\Services\ReviewPageFactory.cs" "Pages\ReviewForm\ReviewFormPageFactory.cs"
Move-File "Features\Review\DependencyInjection\ReviewFeatureServiceCollectionExtensions.cs" "App\Providers\ReviewFeatureServiceCollectionExtensions.cs"

Move-LegacyFeature "Onboarding" "Onboarding" "CompleteOnboarding"
Move-LegacyFeature "Clean" "Audio" "PlayMusic"
Move-LegacyFeature "Motivator" "Quote" "ManageQuotes"
Move-LegacyFeature "Physics" "Physics" "SearchPhysics"
Move-LegacyFeature "Tests" "Test" "RunTests"
Move-LegacyFeature "Profile" "Profile" "ManageProfile"

# Practice special paths
$practice = "Features\Practice"
if (Test-Path $practice) {
    Get-ChildItem "$practice\Ui\Views" -File -Recurse | ForEach-Object {
        $folder = PageFolder $_.BaseName
        Move-File $_.FullName "Pages\$folder\$($_.Name)"
    }
    Get-ChildItem "$practice\Ui\ViewModels" -File -Recurse | ForEach-Object {
        $vm = $_.BaseName -replace '\..*',''
        $pageKey = ($vm -replace 'ViewModel$','') + 'Page'
        $folder = PageFolder $pageKey
        Move-File $_.FullName "Pages\$folder\$($_.Name)"
    }
    Get-ChildItem "$practice\Ui\Components" -File | ForEach-Object {
        Move-File $_.FullName "Widgets\$(WidgetFolder $_.BaseName)\$($_.Name)"
    }
    Get-ChildItem "$practice\Ui\Techniques" -File -Recurse | ForEach-Object {
        if ($_.DirectoryName -match 'Bodies') {
            Move-File $_.FullName "Widgets\TechniqueBodies\$($_.Name)"
        } else {
            Move-File $_.FullName "Features\RunTechniqueSession\$($_.Name)"
        }
    }
    Get-ChildItem "$practice\Model" -File | ForEach-Object {
        Move-File $_.FullName "Entities\Technique\$($_.Name)"
    }
    Get-ChildItem "$practice\Services" -File | ForEach-Object {
        Move-File $_.FullName "Features\RunTechniqueSession\$($_.Name)"
    }
    Get-ChildItem "$practice\DependencyInjection" -File | ForEach-Object {
        Move-File $_.FullName "App\Providers\PracticeFeatureServiceCollectionExtensions.cs"
    }
}

# Shared entities
if (Test-Path "Shared\Model\Quotes\QuoteItem.cs") {
    Move-File "Shared\Model\Quotes\QuoteItem.cs" "Entities\Quote\QuoteItem.cs"
}
if (Test-Path "Shared\Model\FilterChipTabItem.cs") {
    Move-File "Shared\Model\FilterChipTabItem.cs" "Entities\FilterChip\FilterChipTabItem.cs"
}
if (Test-Path "Shared\Controls\TechniquePageShell.xaml") {
    Move-File "Shared\Controls\TechniquePageShell.xaml" "Widgets\TechniqueSessionShell\TechniquePageShell.xaml"
    Move-File "Shared\Controls\TechniquePageShell.xaml.cs" "Widgets\TechniqueSessionShell\TechniquePageShell.xaml.cs"
}

# Move Shared factories to App/Providers temporarily
Ensure-Dir "App\Providers"
Get-ChildItem "Shared\Services\Factories" -File -ErrorAction SilentlyContinue | ForEach-Object {
    Move-File $_.FullName "App\Providers\$($_.Name)"
}

# Move feature DI extensions
@("Motivator","Clean","Physics","Tests","Profile") | ForEach-Object {
    $di = "Features\$_\DependencyInjection\${_}FeatureServiceCollectionExtensions.cs"
    if (Test-Path $di) {
        Move-File $di "App\Providers\${_}FeatureServiceCollectionExtensions.cs"
    }
}

Write-Host "Canonical FSD file moves completed."
