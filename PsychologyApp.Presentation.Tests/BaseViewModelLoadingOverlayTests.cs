using PsychologyApp.Presentation.ViewModels;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class BaseViewModelLoadingOverlayTests
{
    [Fact]
    public void IsLoadingOverlayVisible_IsTrueOnlyDuringInitBeforeDone()
    {
        var viewModel = new LoadingOverlayStubViewModel();

        viewModel.SetInit();
        Assert.True(viewModel.IsLoadingOverlayVisible);

        viewModel.SetDone();
        Assert.False(viewModel.IsLoadingOverlayVisible);
    }

    [Fact]
    public void IsLoadingOverlayVisible_IsFalseWhenOnlyDone()
    {
        var viewModel = new LoadingOverlayStubViewModel();
        viewModel.SetDone();
        Assert.False(viewModel.IsLoadingOverlayVisible);
    }

    private sealed class LoadingOverlayStubViewModel : BaseViewModel;
}
