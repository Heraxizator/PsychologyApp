# Bulk update using/namespace references from legacy Features layout to canonical FSD
$ErrorActionPreference = "Stop"
$roots = @(
    (Join-Path $PSScriptRoot "..\PsychologyApp.Presentation"),
    (Join-Path $PSScriptRoot "..\PsychologyApp.Presentation.Tests")
)

$map = @{
    'PsychologyApp.Presentation.Features.Review.Ui.Views' = 'PsychologyApp.Presentation.Pages.ReviewForm'
    'PsychologyApp.Presentation.Features.Review.Ui.ViewModels' = 'PsychologyApp.Presentation.Pages.ReviewForm'
    'PsychologyApp.Presentation.Features.Review.Services' = 'PsychologyApp.Presentation.Pages.ReviewForm'
    'PsychologyApp.Presentation.Features.Onboarding.Ui.Views' = 'PsychologyApp.Presentation.Pages.Onboarding'
    'PsychologyApp.Presentation.Features.Onboarding.Ui.ViewModels' = 'PsychologyApp.Presentation.Pages.Onboarding'
    'PsychologyApp.Presentation.Features.Onboarding.DependencyInjection' = 'PsychologyApp.Presentation.App.Providers'
    'PsychologyApp.Presentation.Features.Clean.Ui.Views' = 'PsychologyApp.Presentation.Pages.MusicPlayer'
    'PsychologyApp.Presentation.Features.Clean.Ui.ViewModels' = 'PsychologyApp.Presentation.Pages.MusicPlayer'
    'PsychologyApp.Presentation.Features.Clean.Ui.Components' = 'PsychologyApp.Presentation.Widgets.MusicListCard'
    'PsychologyApp.Presentation.Features.Clean.Model' = 'PsychologyApp.Presentation.Entities.Audio'
    'PsychologyApp.Presentation.Features.Clean.Services' = 'PsychologyApp.Presentation.Features.PlayMusic'
    'PsychologyApp.Presentation.Features.Clean.DependencyInjection' = 'PsychologyApp.Presentation.App.Providers'
    'PsychologyApp.Presentation.Features.Motivator.Ui.Views' = 'PsychologyApp.Presentation.Pages.QuoteFeed'
    'PsychologyApp.Presentation.Features.Motivator.Ui.ViewModels' = 'PsychologyApp.Presentation.Pages.QuoteFeed'
    'PsychologyApp.Presentation.Features.Motivator.Ui.Components' = 'PsychologyApp.Presentation.Widgets.QuoteCard'
    'PsychologyApp.Presentation.Features.Motivator.Model' = 'PsychologyApp.Presentation.Entities.Quote'
    'PsychologyApp.Presentation.Features.Motivator.Services' = 'PsychologyApp.Presentation.Features.ManageQuotes'
    'PsychologyApp.Presentation.Features.Motivator.DependencyInjection' = 'PsychologyApp.Presentation.App.Providers'
    'PsychologyApp.Presentation.Features.Physics.Ui.Views' = 'PsychologyApp.Presentation.Pages.PhysicsSearch'
    'PsychologyApp.Presentation.Features.Physics.Ui.ViewModels' = 'PsychologyApp.Presentation.Pages.PhysicsSearch'
    'PsychologyApp.Presentation.Features.Physics.Ui.Components' = 'PsychologyApp.Presentation.Widgets.PhysicsReasonCard'
    'PsychologyApp.Presentation.Features.Physics.Model' = 'PsychologyApp.Presentation.Entities.Physics'
    'PsychologyApp.Presentation.Features.Physics.Services' = 'PsychologyApp.Presentation.Features.SearchPhysics'
    'PsychologyApp.Presentation.Features.Physics.DependencyInjection' = 'PsychologyApp.Presentation.App.Providers'
    'PsychologyApp.Presentation.Features.Tests.Ui.Views' = 'PsychologyApp.Presentation.Pages.TestsList'
    'PsychologyApp.Presentation.Features.Tests.Ui.ViewModels' = 'PsychologyApp.Presentation.Pages.TestsList'
    'PsychologyApp.Presentation.Features.Tests.Ui.Components' = 'PsychologyApp.Presentation.Widgets.TestListCard'
    'PsychologyApp.Presentation.Features.Tests.Model' = 'PsychologyApp.Presentation.Entities.Test'
    'PsychologyApp.Presentation.Features.Tests.Services' = 'PsychologyApp.Presentation.Features.RunTests'
    'PsychologyApp.Presentation.Features.Tests.DependencyInjection' = 'PsychologyApp.Presentation.App.Providers'
    'PsychologyApp.Presentation.Features.Profile.Ui.Views' = 'PsychologyApp.Presentation.Pages.ProfileUser'
    'PsychologyApp.Presentation.Features.Profile.Ui.ViewModels' = 'PsychologyApp.Presentation.Pages.ProfileUser'
    'PsychologyApp.Presentation.Features.Profile.Ui.Components' = 'PsychologyApp.Presentation.Widgets.ProfileTechniqueCard'
    'PsychologyApp.Presentation.Features.Profile.Model' = 'PsychologyApp.Presentation.Entities.Profile'
    'PsychologyApp.Presentation.Features.Profile.Services' = 'PsychologyApp.Presentation.Features.ManageProfile'
    'PsychologyApp.Presentation.Features.Profile.DependencyInjection' = 'PsychologyApp.Presentation.App.Providers'
    'PsychologyApp.Presentation.Features.Practice.Ui.Views' = 'PsychologyApp.Presentation.Pages.Techniques'
    'PsychologyApp.Presentation.Features.Practice.Ui.ViewModels' = 'PsychologyApp.Presentation.Pages.Techniques'
    'PsychologyApp.Presentation.Features.Practice.Ui.ViewModels.Techniques' = 'PsychologyApp.Presentation.Pages.TechniqueSession'
    'PsychologyApp.Presentation.Features.Practice.Ui.ViewModels.Constructor' = 'PsychologyApp.Presentation.Pages.TechniqueDesigner'
    'PsychologyApp.Presentation.Features.Practice.Ui.Views.Techniques' = 'PsychologyApp.Presentation.Pages.TechniqueSession'
    'PsychologyApp.Presentation.Features.Practice.Ui.Views.Constructor' = 'PsychologyApp.Presentation.Pages.TechniqueDesigner'
    'PsychologyApp.Presentation.Features.Practice.Ui.Components' = 'PsychologyApp.Presentation.Widgets.TechniqueListCard'
    'PsychologyApp.Presentation.Features.Practice.Ui.Techniques' = 'PsychologyApp.Presentation.Widgets.TechniqueBodies'
    'PsychologyApp.Presentation.Features.Practice.Model' = 'PsychologyApp.Presentation.Entities.Technique'
    'PsychologyApp.Presentation.Features.Practice.Services' = 'PsychologyApp.Presentation.Features.RunTechniqueSession'
    'PsychologyApp.Presentation.Features.Practice.DependencyInjection' = 'PsychologyApp.Presentation.App.Providers'
    'PsychologyApp.Presentation.Features.Review.DependencyInjection' = 'PsychologyApp.Presentation.App.Providers'
    'PsychologyApp.Presentation.Shared.Model.Quotes' = 'PsychologyApp.Presentation.Entities.Quote'
    'PsychologyApp.Presentation.Shared.Controls' = 'PsychologyApp.Presentation.Widgets.TechniqueSessionShell'
}

foreach ($root in $roots) {
    if (-not (Test-Path $root)) { continue }
    Get-ChildItem -Path $root -Recurse -File -Include *.cs,*.xaml | ForEach-Object {
        $c = Get-Content $_.FullName -Raw
        $orig = $c
        foreach ($k in ($map.Keys | Sort-Object { $_.Length } -Descending)) {
            $c = $c.Replace($k, $map[$k])
        }
        if ($c -ne $orig) { Set-Content -Path $_.FullName -Value $c -NoNewline }
    }
}

Write-Host "Using references updated."
