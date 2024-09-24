using ShoppingList.MVVM.ViewModels;

namespace ShoppingList.MVVM.Views;

public partial class LoginView : ContentPage
{
	public LoginView(LoginViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}     

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var logged = Preferences.Get("UserId", null);

        if (logged != null)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        var vm = BindingContext as LoginViewModel;

        if (vm != null)
        {
            vm.ClearCredentials();
        }
    }
    
}