using Microsoft.Maui.Controls.Shapes;
using PsychologyApp.Presentation.Core.Charts;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.UI.Components;

/// <summary>
/// Grey placeholders in the shape of what is about to appear (cards, rows, chat bubbles, a profile) while a screen loads. They breathe softly
/// (opacity only, no sliding, so it is cheap) and stand still when reduced motion is on. Screen readers hear one word, "Loading", not a list of boxes.
/// </summary>
public sealed class SkeletonView : ContentView
{
    private const string AnimationName = "skeleton-pulse";
    private const double CardRadius = 16;

    public static readonly BindableProperty KindProperty = BindableProperty.Create(
        nameof(Kind), typeof(SkeletonKind), typeof(SkeletonView), SkeletonKind.Cards, propertyChanged: (b, _, _) => ((SkeletonView)b).Rebuild());

    /// <summary>How many repeats of the shape; zero means the usual number for the kind.</summary>
    public static readonly BindableProperty CountProperty = BindableProperty.Create(
        nameof(Count), typeof(int), typeof(SkeletonView), 0, propertyChanged: (b, _, _) => ((SkeletonView)b).Rebuild());

    /// <summary>Bind this instead of IsVisible: turning it off fades the placeholders out while the real content fades in, so there is no empty moment between them.</summary>
    public static readonly BindableProperty IsActiveProperty = BindableProperty.Create(
        nameof(IsActive), typeof(bool), typeof(SkeletonView), false, propertyChanged: (b, _, n) => ((SkeletonView)b).OnActiveChanged((bool)n));

    public SkeletonView()
    {
        IsVisible = false;
        InputTransparent = true;
        SemanticProperties.SetDescription(this, AppStrings.SkeletonLoadingLabel);
        Loaded += (_, _) => { Rebuild(); UpdateMotion(); };
        Unloaded += (_, _) => StopMotion();
        Rebuild();
    }

    public SkeletonKind Kind
    {
        get => (SkeletonKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public int Count
    {
        get => (int)GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    private async void OnActiveChanged(bool active)
    {
        if (active)
        {
            Opacity = 1;
            IsVisible = true;
            return;
        }

        if (ReduceMotion.IsEnabled || Handler is null)
        {
            IsVisible = false;
            return;
        }

        this.AbortAnimation(AnimationName);
        await this.FadeToAsync(0, UiAnimations.ExitRevealDuration, Easing.CubicIn);
        if (!IsActive)
        {
            IsVisible = false;
        }
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == IsVisibleProperty.PropertyName)
        {
            UpdateMotion();
        }
    }

    private void UpdateMotion()
    {
        this.AbortAnimation(AnimationName);
        if (!IsVisible || Handler is null || ReduceMotion.IsEnabled)
        {
            Opacity = IsVisible ? 0.85 : 1;
            return;
        }

        AnimationLayer.Enter(this);
        new Animation(t => Opacity = 1 - 0.45 * Math.Sin(t * Math.PI), 0, 1, Easing.Linear)
            .Commit(this, AnimationName, 16, 1200, Easing.Linear, repeat: () => true);
    }

    private void StopMotion()
    {
        this.AbortAnimation(AnimationName);
        Opacity = 1;
        AnimationLayer.Exit(this);
    }

    private void Rebuild()
    {
        bool dark = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
        Color fill = Color.FromArgb(SkeletonLayout.FillHex(dark));
        Color surface = Color.FromArgb(MoodPalette.Fill(null, dark));
        int count = Count > 0 ? Count : SkeletonLayout.DefaultCount(Kind);

        VerticalStackLayout root = new() { Spacing = 12 };
        foreach (SkeletonGroup group in SkeletonLayout.Build(Kind, count))
        {
            root.Add(BuildGroup(group, fill, surface));
        }

        Content = root;
    }

    private static View BuildGroup(SkeletonGroup group, Color fill, Color surface)
    {
        VerticalStackLayout lines = new() { Spacing = group.InCard ? 8 : 6 };
        foreach (SkeletonBlock block in group.Blocks)
        {
            lines.Add(BuildBlock(block, fill));
        }

        if (group.LeadingCircle)
        {
            Grid row = new() { ColumnSpacing = 12, ColumnDefinitions = [new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star)] };
            View picture = BuildBlock(new SkeletonBlock(0, 40, Circle: true), fill);
            row.Add(picture, 0, 0);
            lines.VerticalOptions = LayoutOptions.Center;
            row.Add(lines, 1, 0);
            return row;
        }

        if (!group.InCard)
        {
            return lines;
        }

        return new Border
        {
            Content = lines,
            Padding = new Thickness(14),
            StrokeThickness = 0,
            BackgroundColor = surface,
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(CardRadius) }
        };
    }

    /// <summary>A grey block. A fraction of the width is a proportional column, so it works at any screen width without measuring.</summary>
    private static View BuildBlock(SkeletonBlock block, Color fill)
    {
        Border shape = new()
        {
            HeightRequest = block.Height,
            StrokeThickness = 0,
            BackgroundColor = fill,
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(block.Circle ? block.Height / 2 : Math.Min(8, block.Height / 2)) }
        };

        if (block.Circle)
        {
            shape.WidthRequest = block.Height;
            shape.HorizontalOptions = block.Align switch
            {
                SkeletonAlign.Center => LayoutOptions.Center,
                SkeletonAlign.End => LayoutOptions.End,
                _ => LayoutOptions.Start
            };
            return shape;
        }

        double f = Math.Clamp(block.WidthFraction, 0.05, 1);
        double rest = 1 - f;
        Grid grid = new();
        switch (block.Align)
        {
            case SkeletonAlign.End:
                grid.ColumnDefinitions = [Star(rest), Star(f)];
                grid.Add(shape, 1, 0);
                break;
            case SkeletonAlign.Center:
                grid.ColumnDefinitions = [Star(rest / 2), Star(f), Star(rest / 2)];
                grid.Add(shape, 1, 0);
                break;
            default:
                grid.ColumnDefinitions = [Star(f), Star(rest)];
                grid.Add(shape, 0, 0);
                break;
        }

        return grid;
    }

    private static ColumnDefinition Star(double weight) => new(new GridLength(Math.Max(weight, 0.0001), GridUnitType.Star));
}
