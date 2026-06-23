# Update namespaces after canonical FSD migration
$ErrorActionPreference = "Stop"
$root = Join-Path $PSScriptRoot "..\PsychologyApp.Presentation"
$tests = Join-Path $PSScriptRoot "..\PsychologyApp.Presentation.Tests"

$replacements = [ordered]@{
    'namespace PsychologyApp.Presentation.Features.Review.Ui.Views' = 'namespace PsychologyApp.Presentation.Pages.ReviewForm'
    'namespace PsychologyApp.Presentation.Features.Review.Ui.ViewModels' = 'namespace PsychologyApp.Presentation.Pages.ReviewForm'
    'namespace PsychologyApp.Presentation.Features.Review.Services' = 'namespace PsychologyApp.Presentation.Pages.ReviewForm'
    'namespace PsychologyApp.Presentation.Features.Review.DependencyInjection' = 'namespace PsychologyApp.Presentation.App.Providers'
    'namespace PsychologyApp.Presentation.Features.Onboarding.Ui.Views' = 'namespace PsychologyApp.Presentation.Pages.Onboarding'
    'namespace PsychologyApp.Presentation.Features.Onboarding.Ui.ViewModels' = 'namespace PsychologyApp.Presentation.Pages.Onboarding'
    'namespace PsychologyApp.Presentation.Features.Onboarding.DependencyInjection' = 'namespace PsychologyApp.Presentation.App.Providers'
    'namespace PsychologyApp.Presentation.Features.Clean.Ui.Views' = 'namespace PsychologyApp.Presentation.Pages.MusicPlayer'
    'namespace PsychologyApp.Presentation.Features.Clean.Ui.ViewModels' = 'namespace PsychologyApp.Presentation.Pages.MusicPlayer'
    'namespace PsychologyApp.Presentation.Features.Clean.Ui.Components' = 'namespace PsychologyApp.Presentation.Widgets.MusicListCard'
    'namespace PsychologyApp.Presentation.Features.Clean.Model' = 'namespace PsychologyApp.Presentation.Entities.Audio'
    'namespace PsychologyApp.Presentation.Features.Clean.Services' = 'namespace PsychologyApp.Presentation.Features.PlayMusic'
    'namespace PsychologyApp.Presentation.Features.Clean.DependencyInjection' = 'namespace PsychologyApp.Presentation.App.Providers'
    'namespace PsychologyApp.Presentation.Features.Motivator.Ui.Views' = 'namespace PsychologyApp.Presentation.Pages.QuoteFeed'
    'namespace PsychologyApp.Presentation.Features.Motivator.Ui.ViewModels' = 'namespace PsychologyApp.Presentation.Pages.QuoteFeed'
    'namespace PsychologyApp.Presentation.Features.Motivator.Ui.Components' = 'namespace PsychologyApp.Presentation.Widgets.QuoteCard'
    'namespace PsychologyApp.Presentation.Features.Motivator.Model' = 'namespace PsychologyApp.Presentation.Entities.Quote'
    'namespace PsychologyApp.Presentation.Features.Motivator.Services' = 'namespace PsychologyApp.Presentation.Features.ManageQuotes'
    'namespace PsychologyApp.Presentation.Features.Motivator.DependencyInjection' = 'namespace PsychologyApp.Presentation.App.Providers'
    'namespace PsychologyApp.Presentation.Features.Physics.Ui.Views' = 'namespace PsychologyApp.Presentation.Pages.PhysicsSearch'
    'namespace PsychologyApp.Presentation.Features.Physics.Ui.ViewModels' = 'namespace PsychologyApp.Presentation.Pages.PhysicsSearch'
    'namespace PsychologyApp.Presentation.Features.Physics.Ui.Components' = 'namespace PsychologyApp.Presentation.Widgets.PhysicsReasonCard'
    'namespace PsychologyApp.Presentation.Features.Physics.Model' = 'namespace PsychologyApp.Presentation.Entities.Physics'
    'namespace PsychologyApp.Presentation.Features.Physics.Services' = 'namespace PsychologyApp.Presentation.Features.SearchPhysics'
    'namespace PsychologyApp.Presentation.Features.Physics.DependencyInjection' = 'namespace PsychologyApp.Presentation.App.Providers'
    'namespace PsychologyApp.Presentation.Features.Tests.Ui.Views' = 'namespace PsychologyApp.Presentation.Pages.TestsList'
    'namespace PsychologyApp.Presentation.Features.Tests.Ui.ViewModels' = 'namespace PsychologyApp.Presentation.Pages.TestsList'
    'namespace PsychologyApp.Presentation.Features.Tests.Ui.Components' = 'namespace PsychologyApp.Presentation.Widgets.TestListCard'
    'namespace PsychologyApp.Presentation.Features.Tests.Model' = 'namespace PsychologyApp.Presentation.Entities.Test'
    'namespace PsychologyApp.Presentation.Features.Tests.Services' = 'namespace PsychologyApp.Presentation.Features.RunTests'
    'namespace PsychologyApp.Presentation.Features.Tests.DependencyInjection' = 'namespace PsychologyApp.Presentation.App.Providers'
    'namespace PsychologyApp.Presentation.Features.Profile.Ui.Views' = 'namespace PsychologyApp.Presentation.Pages.ProfileUser'
    'namespace PsychologyApp.Presentation.Features.Profile.Ui.ViewModels' = 'namespace PsychologyApp.Presentation.Pages.ProfileUser'
    'namespace PsychologyApp.Presentation.Features.Profile.Ui.Components' = 'namespace PsychologyApp.Presentation.Widgets.ProfileTechniqueCard'
    'namespace PsychologyApp.Presentation.Features.Profile.Model' = 'namespace PsychologyApp.Presentation.Entities.Profile'
    'namespace PsychologyApp.Presentation.Features.Profile.Services' = 'namespace PsychologyApp.Presentation.Features.ManageProfile'
    'namespace PsychologyApp.Presentation.Features.Profile.DependencyInjection' = 'namespace PsychologyApp.Presentation.App.Providers'
    'namespace PsychologyApp.Presentation.Features.Practice.Ui.Views' = 'namespace PsychologyApp.Presentation.Pages.Techniques'
    'namespace PsychologyApp.Presentation.Features.Practice.Ui.ViewModels' = 'namespace PsychologyApp.Presentation.Pages.Techniques'
    'namespace PsychologyApp.Presentation.Features.Practice.Ui.Components' = 'namespace PsychologyApp.Presentation.Widgets.TechniqueListCard'
    'namespace PsychologyApp.Presentation.Features.Practice.Ui.Techniques' = 'namespace PsychologyApp.Presentation.Widgets.TechniqueBodies'
    'namespace PsychologyApp.Presentation.Features.Practice.Model' = 'namespace PsychologyApp.Presentation.Entities.Technique'
    'namespace PsychologyApp.Presentation.Features.Practice.Services' = 'namespace PsychologyApp.Presentation.Features.RunTechniqueSession'
    'namespace PsychologyApp.Presentation.Features.Practice.DependencyInjection' = 'namespace PsychologyApp.Presentation.App.Providers'
    'namespace PsychologyApp.Presentation.Shared.Model.Quotes' = 'namespace PsychologyApp.Presentation.Entities.Quote'
    'namespace PsychologyApp.Presentation.Shared.Model' = 'namespace PsychologyApp.Presentation.Entities.FilterChip'
    'namespace PsychologyApp.Presentation.Shared.Controls' = 'namespace PsychologyApp.Presentation.Widgets.TechniqueSessionShell'
}

$usingReplacements = [ordered]@{
    'using PsychologyApp.Presentation.Features.Review.Ui.Views' = 'using PsychologyApp.Presentation.Pages.ReviewForm'
    'using PsychologyApp.Presentation.Features.Review.Ui.ViewModels' = 'using PsychologyApp.Presentation.Pages.ReviewForm'
    'using PsychologyApp.Presentation.Features.Review.Services' = 'using PsychologyApp.Presentation.Pages.ReviewForm'
    'using PsychologyApp.Presentation.Features.Onboarding.Ui.Views' = 'using PsychologyApp.Presentation.Pages.Onboarding'
    'using PsychologyApp.Presentation.Features.Onboarding.Ui.ViewModels' = 'using PsychologyApp.Presentation.Pages.Onboarding'
    'using PsychologyApp.Presentation.Features.Clean.Ui.Views' = 'using PsychologyApp.Presentation.Pages.MusicPlayer'
    'using PsychologyApp.Presentation.Features.Clean.Ui.ViewModels' = 'using PsychologyApp.Presentation.Pages.MusicPlayer'
    'using PsychologyApp.Presentation.Features.Clean.Ui.Components' = 'using PsychologyApp.Presentation.Widgets.MusicListCard'
    'using PsychologyApp.Presentation.Features.Clean.Model' = 'using PsychologyApp.Presentation.Entities.Audio'
    'using PsychologyApp.Presentation.Features.Clean.Services' = 'using PsychologyApp.Presentation.Features.PlayMusic'
    'using PsychologyApp.Presentation.Features.Motivator.Ui.Views' = 'using PsychologyApp.Presentation.Pages.QuoteFeed'
    'using PsychologyApp.Presentation.Features.Motivator.Ui.ViewModels' = 'using PsychologyApp.Presentation.Pages.QuoteFeed'
    'using PsychologyApp.Presentation.Features.Motivator.Ui.Components' = 'using PsychologyApp.Presentation.Widgets.QuoteCard'
    'using PsychologyApp.Presentation.Features.Motivator.Model' = 'using PsychologyApp.Presentation.Entities.Quote'
    'using PsychologyApp.Presentation.Features.Motivator.Services' = 'using PsychologyApp.Presentation.Features.ManageQuotes'
    'using PsychologyApp.Presentation.Features.Physics.Ui.Views' = 'using PsychologyApp.Presentation.Pages.PhysicsSearch'
    'using PsychologyApp.Presentation.Features.Physics.Ui.ViewModels' = 'using PsychologyApp.Presentation.Pages.PhysicsSearch'
    'using PsychologyApp.Presentation.Features.Physics.Ui.Components' = 'using PsychologyApp.Presentation.Widgets.PhysicsReasonCard'
    'using PsychologyApp.Presentation.Features.Physics.Model' = 'using PsychologyApp.Presentation.Entities.Physics'
    'using PsychologyApp.Presentation.Features.Physics.Services' = 'using PsychologyApp.Presentation.Features.SearchPhysics'
    'using PsychologyApp.Presentation.Features.Tests.Ui.Views' = 'using PsychologyApp.Presentation.Pages.TestsList'
    'using PsychologyApp.Presentation.Features.Tests.Ui.ViewModels' = 'using PsychologyApp.Presentation.Pages.TestsList'
    'using PsychologyApp.Presentation.Features.Tests.Ui.Components' = 'using PsychologyApp.Presentation.Widgets.TestListCard'
    'using PsychologyApp.Presentation.Features.Tests.Model' = 'using PsychologyApp.Presentation.Entities.Test'
    'using PsychologyApp.Presentation.Features.Tests.Services' = 'using PsychologyApp.Presentation.Features.RunTests'
    'using PsychologyApp.Presentation.Features.Profile.Ui.Views' = 'using PsychologyApp.Presentation.Pages.ProfileUser'
    'using PsychologyApp.Presentation.Features.Profile.Ui.ViewModels' = 'using PsychologyApp.Presentation.Pages.ProfileUser'
    'using PsychologyApp.Presentation.Features.Profile.Ui.Components' = 'using PsychologyApp.Presentation.Widgets.ProfileTechniqueCard'
    'using PsychologyApp.Presentation.Features.Profile.Model' = 'using PsychologyApp.Presentation.Entities.Profile'
    'using PsychologyApp.Presentation.Features.Profile.Services' = 'using PsychologyApp.Presentation.Features.ManageProfile'
    'using PsychologyApp.Presentation.Features.Practice.Ui.Views' = 'using PsychologyApp.Presentation.Pages.Techniques'
    'using PsychologyApp.Presentation.Features.Practice.Ui.ViewModels' = 'using PsychologyApp.Presentation.Pages.Techniques'
    'using PsychologyApp.Presentation.Features.Practice.Ui.Components' = 'using PsychologyApp.Presentation.Widgets.TechniqueListCard'
    'using PsychologyApp.Presentation.Features.Practice.Ui.Techniques' = 'using PsychologyApp.Presentation.Widgets.TechniqueBodies'
    'using PsychologyApp.Presentation.Features.Practice.Model' = 'using PsychologyApp.Presentation.Entities.Technique'
    'using PsychologyApp.Presentation.Features.Practice.Services' = 'using PsychologyApp.Presentation.Features.RunTechniqueSession'
    'using PsychologyApp.Presentation.Shared.Model.Quotes' = 'using PsychologyApp.Presentation.Entities.Quote'
    'using PsychologyApp.Presentation.Shared.Model' = 'using PsychologyApp.Presentation.Entities.FilterChip'
    'using PsychologyApp.Presentation.Shared.Controls' = 'using PsychologyApp.Presentation.Widgets.TechniqueSessionShell'
    'using PsychologyApp.Presentation.Features.Review.DependencyInjection' = 'using PsychologyApp.Presentation.App.Providers'
    'using PsychologyApp.Presentation.Features.Onboarding.DependencyInjection' = 'using PsychologyApp.Presentation.App.Providers'
    'using PsychologyApp.Presentation.Features.Clean.DependencyInjection' = 'using PsychologyApp.Presentation.App.Providers'
    'using PsychologyApp.Presentation.Features.Motivator.DependencyInjection' = 'using PsychologyApp.Presentation.App.Providers'
    'using PsychologyApp.Presentation.Features.Physics.DependencyInjection' = 'using PsychologyApp.Presentation.App.Providers'
    'using PsychologyApp.Presentation.Features.Tests.DependencyInjection' = 'using PsychologyApp.Presentation.App.Providers'
    'using PsychologyApp.Presentation.Features.Profile.DependencyInjection' = 'using PsychologyApp.Presentation.App.Providers'
    'using PsychologyApp.Presentation.Features.Practice.DependencyInjection' = 'using PsychologyApp.Presentation.App.Providers'
}

function Update-Files($path, $patterns) {
    Get-ChildItem $path -Recurse -Include $patterns | ForEach-Object {
        $c = Get-Content $_.FullName -Raw
        $orig = $c
        foreach ($k in $replacements.Keys) { $c = $c.Replace($k, $replacements[$k]) }
        foreach ($k in $usingReplacements.Keys) { $c = $c.Replace($k, $usingReplacements[$k]) }
        if ($c -ne $orig) { Set-Content -Path $_.FullName -Value $c -NoNewline }
    }
}

Update-Files $root @('*.cs')
Update-Files $root @('*.xaml','*.xaml.cs')
if (Test-Path $tests) { Update-Files $tests @('*.cs') }

Write-Host "Namespace updates completed."
