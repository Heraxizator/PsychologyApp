using Microsoft.Maui.ApplicationModel;
using Moq;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Pages.ClinicalCare.SafetyPlan;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

/// <summary>The safety plan is written by the person for a bad moment; losing or mangling an entry would be worse than any crash.</summary>
[Collection("StaticShims")]
public sealed class SafetyPlanViewModelTests
{
    private readonly Mock<INavigationService> _navigation = new();
    private readonly Mock<IClinicalCareService> _care = new();
    private readonly List<SafetyPlanDTO> _saved = [];

    public SafetyPlanViewModelTests()
    {
        Launcher.Opened.Clear();
        Launcher.FailWith = null;
        _navigation.Setup(n => n.GoBackAsync()).Returns(Task.CompletedTask);
        _care.Setup(c => c.SaveSafetyPlanAsync(It.IsAny<SafetyPlanDTO>(), It.IsAny<CancellationToken>()))
            .Callback<SafetyPlanDTO, CancellationToken>((plan, _) => _saved.Add(plan))
            .Returns(Task.CompletedTask);
    }

    private async Task<SafetyPlanViewModel> CreateAsync(SafetyPlanDTO? plan = null)
    {
        _care.Setup(c => c.GetSafetyPlanAsync(It.IsAny<CancellationToken>())).ReturnsAsync(plan ?? new SafetyPlanDTO());
        SafetyPlanViewModel viewModel = new(_navigation.Object, _care.Object);
        await VmTestHelpers.WaitUntilAsync(() => _care.Invocations.Any(i => i.Method.Name == nameof(IClinicalCareService.GetSafetyPlanAsync)));
        await Task.Delay(30);
        return viewModel;
    }

    [Fact]
    public async Task TheSavedPlanIsShownWhenTheScreenOpens()
    {
        SafetyPlanViewModel viewModel = await CreateAsync(new SafetyPlanDTO
        {
            WarningSigns = ["can't sleep"],
            CopingStrategies = ["walk", "tea"],
            Reasons = ["my dog"],
            Contacts = [new SafetyPlanContactDTO { Name = "Anna", Phone = "+7 900" }]
        });

        Assert.Equal(["can't sleep"], viewModel.WarningSigns.Select(i => i.Text));
        Assert.Equal(["walk", "tea"], viewModel.CopingStrategies.Select(i => i.Text));
        Assert.Equal(["my dog"], viewModel.Reasons.Select(i => i.Text));
        Assert.Equal("Anna", Assert.Single(viewModel.Contacts).Name);
    }

    [Fact]
    public async Task AddingAnEntryTrimsItClearsTheFieldAndSavesTheWholePlan()
    {
        SafetyPlanViewModel viewModel = await CreateAsync(new SafetyPlanDTO { Reasons = ["family"] });
        viewModel.NewCopingStrategyText = "  call a friend  ";

        await VmTestHelpers.RunAsync(viewModel.AddCopingStrategyCommand);

        Assert.Equal(string.Empty, viewModel.NewCopingStrategyText);
        SafetyPlanDTO saved = Assert.Single(_saved);
        Assert.Equal(["call a friend"], saved.CopingStrategies);
        Assert.Equal(["family"], saved.Reasons);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ABlankEntryIsIgnoredAndNothingIsSaved(string text)
    {
        SafetyPlanViewModel viewModel = await CreateAsync();
        viewModel.NewWarningSignText = text;

        await VmTestHelpers.RunAsync(viewModel.AddWarningSignCommand);

        Assert.Empty(viewModel.WarningSigns);
        Assert.Empty(_saved);
    }

    [Fact]
    public async Task RemovingAnEntryRemovesOnlyThatEntryAndSaves()
    {
        SafetyPlanViewModel viewModel = await CreateAsync(new SafetyPlanDTO { WarningSigns = ["a", "b", "c"] });
        SafetyPlanTextItem middle = viewModel.WarningSigns[1];

        await VmTestHelpers.RunAsync(middle.DeleteCommand);

        Assert.Equal(["a", "c"], viewModel.WarningSigns.Select(i => i.Text));
        Assert.Equal(["a", "c"], Assert.Single(_saved).WarningSigns);
    }

    [Fact]
    public async Task AContactNeedsANameOrAPhone()
    {
        SafetyPlanViewModel viewModel = await CreateAsync();

        await VmTestHelpers.RunAsync(viewModel.AddContactCommand);
        Assert.Empty(viewModel.Contacts);

        viewModel.NewContactPhone = " 112 ";
        await VmTestHelpers.RunAsync(viewModel.AddContactCommand);

        SafetyPlanContactItem contact = Assert.Single(viewModel.Contacts);
        Assert.Equal("112", contact.Phone);
        Assert.Equal(string.Empty, viewModel.NewContactPhone);
        Assert.Equal("112", Assert.Single(Assert.Single(_saved).Contacts).Phone);
    }

    [Fact]
    public async Task TheCallButtonDialsTheContactAndAnEmptyNumberDoesNothing()
    {
        SafetyPlanViewModel viewModel = await CreateAsync(new SafetyPlanDTO
        {
            Contacts = [new SafetyPlanContactDTO { Name = "Anna", Phone = "+7 900" }, new SafetyPlanContactDTO { Name = "No number", Phone = "" }]
        });

        await VmTestHelpers.RunAsync(viewModel.Contacts[0].CallCommand);
        await VmTestHelpers.RunAsync(viewModel.Contacts[1].CallCommand);

        Assert.Equal(["tel:+7 900"], Launcher.Opened);
    }

    [Fact]
    public async Task ADialIntentThatFailsDoesNotCrashTheScreen()
    {
        SafetyPlanViewModel viewModel = await CreateAsync(new SafetyPlanDTO { Contacts = [new SafetyPlanContactDTO { Name = "Anna", Phone = "1" }] });
        Launcher.FailWith = new InvalidOperationException("no dialer");

        await VmTestHelpers.RunAsync(viewModel.Contacts[0].CallCommand);

        Assert.True(viewModel.Contacts[0].CallCommand.CanExecute(null));
    }
}
