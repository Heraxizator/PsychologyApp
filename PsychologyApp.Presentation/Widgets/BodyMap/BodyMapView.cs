using System.Windows.Input;
using PsychologyApp.Application.Body;
using PsychologyApp.Presentation.Body;

namespace PsychologyApp.Presentation.Widgets.BodyMap;

/// <summary>
/// The front silhouette: a tap on a place runs <see cref="TapCommand"/> with it, the chosen place is drawn in the accent colour. Screen
/// readers skip it; the same places are buttons in the list under it.
/// </summary>
public sealed class BodyMapView : GraphicsView
{
    public static readonly BindableProperty SelectedAreaProperty = BindableProperty.Create(
        nameof(SelectedArea),
        typeof(BodyArea?),
        typeof(BodyMapView),
        null,
        propertyChanged: (bindable, _, newValue) =>
        {
            BodyMapView view = (BodyMapView)bindable;
            view._drawable.Selected = (BodyArea?)newValue;
            view.Invalidate();
        });

    public static readonly BindableProperty TapCommandProperty = BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(BodyMapView));

    private readonly BodyMapDrawable _drawable = new();

    public BodyMapView()
    {
        Drawable = _drawable;
        BackgroundColor = Colors.Transparent;
        AutomationProperties.SetIsInAccessibleTree(this, false);

        TapGestureRecognizer tap = new();
        tap.Tapped += OnTapped;
        GestureRecognizers.Add(tap);

        Loaded += (_, _) =>
        {
            if (Microsoft.Maui.Controls.Application.Current is { } app)
            {
                app.RequestedThemeChanged += OnThemeChanged;
            }
        };
        Unloaded += (_, _) =>
        {
            if (Microsoft.Maui.Controls.Application.Current is { } app)
            {
                app.RequestedThemeChanged -= OnThemeChanged;
            }
        };
    }

    public BodyArea? SelectedArea
    {
        get => (BodyArea?)GetValue(SelectedAreaProperty);
        set => SetValue(SelectedAreaProperty, value);
    }

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    private void OnThemeChanged(object? sender, AppThemeChangedEventArgs e) => Invalidate();

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        if (e.GetPosition(this) is not { } point || BodyMapLayout.AreaAt(point.X, point.Y, Width, Height) is not { } area)
        {
            return;
        }

        if (TapCommand?.CanExecute(area) == true)
        {
            TapCommand.Execute(area);
        }
    }

    private sealed class BodyMapDrawable : IDrawable
    {
        private const float Corner = 7;

        public BodyArea? Selected { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            bool dark = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
            Color idle = Resolve(dark ? "PrimaryTintDark" : "PrimaryTint", Colors.LightBlue);
            Color chosen = Resolve("PrimaryFill", Colors.RoyalBlue);
            Color outline = Resolve(dark ? "BorderSubtleDark" : "BorderSubtleLight", Colors.LightGray);

            (double scale, double offsetX, double offsetY) = BodyMapLayout.Fit(dirtyRect.Width, dirtyRect.Height);
            canvas.StrokeSize = 1;
            canvas.StrokeColor = outline;

            // The face sits on the head: drawn after it, it stays visible as the part of the head one can pick.
            foreach (BodyMapShape shape in BodyMapLayout.Shapes)
            {
                RectF rect = new(
                    (float)(offsetX + shape.X * scale),
                    (float)(offsetY + shape.Y * scale),
                    (float)(shape.Width * scale),
                    (float)(shape.Height * scale));
                bool whole = Selected == BodyArea.WholeBody;
                canvas.FillColor = shape.Area == Selected || whole ? chosen : idle;
                if (shape.IsEllipse)
                {
                    canvas.FillEllipse(rect);
                    canvas.DrawEllipse(rect);
                }
                else
                {
                    canvas.FillRoundedRectangle(rect, Corner);
                    canvas.DrawRoundedRectangle(rect, Corner);
                }
            }
        }

        private static Color Resolve(string key, Color fallback) =>
            Microsoft.Maui.Controls.Application.Current?.Resources.TryGetValue(key, out object? value) == true && value is Color color ? color : fallback;
    }
}
