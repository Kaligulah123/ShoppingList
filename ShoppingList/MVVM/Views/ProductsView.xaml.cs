using ShoppingList.MVVM.Models;
using ShoppingList.MVVM.ViewModels;

namespace ShoppingList.MVVM.Views;

public partial class ProductsView : ContentPage
{
	public ProductsView(MainPageViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var vm = BindingContext as MainPageViewModel;

        if (vm != null)
        {
            vm.ProductsList.Clear();            
         
            await Task.Delay(500);

            await vm.GetProducts();
        }      
    }  
}