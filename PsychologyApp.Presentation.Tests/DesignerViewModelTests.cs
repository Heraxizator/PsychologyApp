using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Practice;
using PsychologyApp.Presentation.ViewModels.Practice.Constructor;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class DesignerViewModelTests
{
    [Fact]
    public async Task ExecuteTechnique_AddsTechniqueAndNavigatesBack()
    {
        Mock<ITechniqueService> techniqueService = new();
        techniqueService
            .Setup(t => t.AddNewTechniqueAsync(It.IsAny<TechniqueDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        Mock<ITechniqueMessenger> messenger = new();
        Mock<INavigationService> navigation = new();
        navigation.Setup(n => n.GoBackAsync()).Returns(Task.CompletedTask);
        IOptions<AppSettings> settings = Options.Create(new AppSettings());

        DesignerViewModel viewModel = CreateViewModel(
            techniqueId: -1,
            techniqueService.Object,
            messenger.Object,
            navigation.Object,
            settings);

        viewModel.Name = "Test technique";
        viewModel.Description = "Description";
        viewModel.Theme = "Theme";
        viewModel.Author = "Author";
        viewModel.Actions = "Step 1";

        await Task.Delay(100);
        viewModel.ExecuteTechnique.Execute(null);
        await Task.Delay(200);

        techniqueService.Verify(
            t => t.AddNewTechniqueAsync(
                It.Is<TechniqueDTO>(dto => dto.Header == "Test technique" && dto.Description == "Description"),
                It.IsAny<CancellationToken>()),
            Times.Once);
        messenger.Verify(
            m => m.Send(It.Is<TechniqueMessage>(msg => msg.MessageType == TechniqueMessageType.Add)),
            Times.Once);
        navigation.Verify(n => n.GoBackAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteTechnique_UpdatesTechniqueAndNavigatesToRoot()
    {
        Mock<ITechniqueService> techniqueService = new();
        techniqueService
            .Setup(t => t.UpdateTechniqueAsync(It.IsAny<TechniqueDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        Mock<ITechniqueMessenger> messenger = new();
        Mock<INavigationService> navigation = new();
        navigation.Setup(n => n.GoToRootAsync()).Returns(Task.CompletedTask);
        IOptions<AppSettings> settings = Options.Create(new AppSettings());

        DesignerViewModel viewModel = CreateViewModel(
            techniqueId: 42,
            techniqueService.Object,
            messenger.Object,
            navigation.Object,
            settings);

        viewModel.Name = "Updated";
        viewModel.Description = "Updated description";
        viewModel.Theme = "Theme";
        viewModel.Author = "Author";
        viewModel.Actions = "Step 1";

        viewModel.ExecuteTechnique.Execute(null);
        await Task.Delay(200);

        techniqueService.Verify(
            t => t.UpdateTechniqueAsync(
                It.Is<TechniqueDTO>(dto => dto.TechniqueId == 42 && dto.Header == "Updated"),
                It.IsAny<CancellationToken>()),
            Times.Once);
        messenger.Verify(
            m => m.Send(It.Is<TechniqueMessage>(msg => msg.MessageType == TechniqueMessageType.Change)),
            Times.Once);
        navigation.Verify(n => n.GoToRootAsync(), Times.Once);
    }

    private static DesignerViewModel CreateViewModel(
        long techniqueId,
        ITechniqueService techniqueService,
        ITechniqueMessenger messenger,
        INavigationService navigation,
        IOptions<AppSettings> settings) =>
        new(
            techniqueId,
            techniqueService,
            messenger,
            new DesignerTechniqueOperations(),
            NullLogger<DesignerViewModel>.Instance,
            settings,
            navigation);
}

public sealed class DesignerTechniqueOperationsTests
{
    [Fact]
    public void BuildDto_MapsFormFields()
    {
        DesignerTechniqueOperations operations = new();
        DesignerTechniqueForm form = new("Name", "Desc", "Theme", "Author", "Actions", "image.png");

        TechniqueDTO dto = operations.BuildDto(5, form);

        Assert.Equal(5, dto.TechniqueId);
        Assert.Equal("Name", dto.Header);
        Assert.Equal("Desc", dto.Description);
        Assert.Equal("Theme", dto.Subject);
        Assert.Equal("Author", dto.Author);
        Assert.Equal("Actions", dto.Algorithm);
        Assert.Equal("image.png", dto.Image);
        Assert.False(string.IsNullOrWhiteSpace(dto.Number));
        Assert.False(string.IsNullOrWhiteSpace(dto.Date));
    }

    [Fact]
    public async Task UpdateAsync_SendsChangeMessage()
    {
        Mock<ITechniqueService> techniqueService = new();
        techniqueService
            .Setup(s => s.UpdateTechniqueAsync(It.IsAny<TechniqueDTO>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        Mock<ITechniqueMessenger> messenger = new();
        DesignerTechniqueOperations operations = new();
        TechniqueDTO dto = operations.BuildDto(7, new DesignerTechniqueForm("A", null, null, null, null, null));

        await operations.UpdateAsync(dto, techniqueService.Object, messenger.Object, CancellationToken.None);

        messenger.Verify(
            m => m.Send(It.Is<TechniqueMessage>(msg => msg.MessageType == TechniqueMessageType.Change && msg.Technique.TechniqueId == 7)),
            Times.Once);
    }
}
