using PsychologyApp.Presentation.Modules.Tester.Collection;

namespace MobileHelperMaui.Views.TestPages;

public partial class TestsListPage : ContentPage
{
    private readonly TestsListViewModel ViewModel;
    public TestsListPage()
    {
        InitializeComponent();

        TestsListViewModel vm = new(Navigation);
        ViewModel = vm;
        BindingContext = vm;
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not TestItem testItem)
        {
            return;
        }

        await Navigation.PushAsync(new FindProblemPage(testItem.Description, testItem.Algorithm, testItem.Comment, testItem.Action), false);

        coll_view.SelectedItem = null;
    }
}