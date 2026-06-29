using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.Navigation;

public interface IPageViewModelActivator
{
    T Bind<T>(ContentPage page, Func<ContentPage, T> factory) where T : class;
}

public sealed class PageViewModelActivator : IPageViewModelActivator
{
    public T Bind<T>(ContentPage page, Func<ContentPage, T> factory) where T : class
    {
        T viewModel = factory(page);
        page.BindingContext = viewModel;
        return viewModel;
    }
}

public static class PageActivationExtensions
{
    public static T ActivateViewModel<T>(
        this ContentPage page,
        IPageViewModelActivator activator,
        Func<ContentPage, T> factory) where T : class =>
        activator.Bind(page, factory);
}
