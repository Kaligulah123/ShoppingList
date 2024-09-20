using ShoppingList.MVVM.ViewModels;

namespace ShoppingList.MVVM.Views;

public partial class LoginView : ContentPage
{
	public LoginView(LoginViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

		var vm = BindingContext as LoginViewModel;

		if (vm != null)
		{
			vm.ClearCredentials();
		}
    }
}