using ShoppingList.MVVM.ViewModels;

namespace ShoppingList.MVVM.Views;

public partial class ProductsView : ContentPage
{
	public ProductsView(MainPageViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var vm = BindingContext as MainPageViewModel;

        if (vm != null)
        {
            vm.ProductsList.Clear();

            vm.GetProducts();
        }      
    }
}