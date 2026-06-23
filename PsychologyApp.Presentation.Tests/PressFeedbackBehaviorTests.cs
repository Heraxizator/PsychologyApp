using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Behaviors;
using System.Windows.Input;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class PressFeedbackBehaviorTests
{
    [Fact]
    public void FindTapTargets_IncludesBorderWithTapCommand()
    {
        var border = new Border();
        border.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { }) });
        var host = new Grid();
        host.Children.Add(border);

        List<View> targets = PressFeedbackTapScanner.FindTapTargets(host).ToList();

        Assert.Contains(border, targets);
    }

    [Fact]
    public void FindTapTargets_SkipsEntryEvenWithTapCommand()
    {
        var entry = new Entry();
        entry.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { }) });
        var host = new Grid();
        host.Children.Add(entry);

        List<View> targets = PressFeedbackTapScanner.FindTapTargets(host).ToList();

        Assert.DoesNotContain(entry, targets);
    }

    [Fact]
    public void FindTapTargets_SkipsTapWithoutCommand()
    {
        var border = new Border();
        border.GestureRecognizers.Add(new TapGestureRecognizer());
        var host = new Grid();
        host.Children.Add(border);

        List<View> targets = PressFeedbackTapScanner.FindTapTargets(host).ToList();

        Assert.Empty(targets);
    }

    [Fact]
    public void IsAttached_ReturnsTrueAfterAttach()
    {
        var border = new Border();

        VisualElementPressFeedback.Attach(border);

        Assert.True(VisualElementPressFeedback.IsAttached(border));
    }

    [Fact]
    public void AttachToPage_AddsBehaviorOnce()
    {
        var page = new ContentPage
        {
            Content = new Grid()
        };

        PressFeedbackHost.AttachToPage(page);
        PressFeedbackHost.AttachToPage(page);

        Grid root = (Grid)page.Content;
        int behaviorCount = root.Behaviors.OfType<PressFeedbackBehavior>().Count(b => b.AttachToAllTapTargets);

        Assert.Equal(1, behaviorCount);
    }

    [Fact]
    public void Detach_RemovesAttachment()
    {
        var border = new Border();
        VisualElementPressFeedback.Attach(border);
        VisualElementPressFeedback.Detach(border);

        Assert.False(VisualElementPressFeedback.IsAttached(border));
    }

    [Fact]
    public void AttachToPage_WhenContentSetLater_AddsBehavior()
    {
        var page = new ContentPage();

        PressFeedbackHost.AttachToPage(page);
        page.Content = new Grid();
        PressFeedbackHost.AttachToPage(page);

        Grid root = (Grid)page.Content;
        Assert.Contains(root.Behaviors, b => b is PressFeedbackBehavior { AttachToAllTapTargets: true });
    }
}
