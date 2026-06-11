using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Services;

public interface IPageViewModelActivator
{
    T Bind<T>(ContentPage page, Func<INavigation, T> factory) where T : class;
}

public sealed class PageViewModelActivator : IPageViewModelActivator
{
    public T Bind<T>(ContentPage page, Func<INavigation, T> factory) where T : class
    {
        T viewModel = factory(page.Navigation);
        page.BindingContext = viewModel;
        return viewModel;
    }
}

public static class PageActivationExtensions
{
    public static T ActivateViewModel<T>(
        this ContentPage page,
        IPageViewModelActivator activator,
        Func<INavigation, T> factory) where T : class =>
        activator.Bind(page, factory);
}
