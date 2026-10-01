namespace PsychologyApp.Presentation.Shared.UI.Components;

/// <summary>
/// An Image that only exists once a card actually shows a picture. Hidden views still get a native view and
/// handler in MAUI, so a collapsed Image in every list row cost an ImageView per row although only techniques the
/// user designed have a photo.
/// </summary>
internal sealed class LazyImageSlot(double size)
{
    private Layout? _host;
    private Image? _image;

    public void Attach(Layout? host)
    {
        _host = host;
        _image = null;
    }

    public void Show(string? source, bool visible)
    {
        if (!visible || string.IsNullOrWhiteSpace(source))
        {
            if (_image is not null)
            {
                _image.IsVisible = false;
            }

            return;
        }

        if (_host is null)
        {
            return;
        }

        if (_image is null)
        {
            _image = new Image
            {
                WidthRequest = size,
                HeightRequest = size,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            _host.Children.Add(_image);
        }

        _image.Source = source;
        _image.IsVisible = true;
    }
}
