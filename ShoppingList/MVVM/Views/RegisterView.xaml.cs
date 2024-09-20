using ShoppingList.MVVM.ViewModels;

namespace ShoppingList.MVVM.Views;

public partial class RegisterView : ContentPage
{
	public RegisterView(LoginViewModel vm)
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