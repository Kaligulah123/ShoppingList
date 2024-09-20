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

    private void checkBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        //var isBusy = false;

        //if (isBusy) return;

        //isBusy = true;

        //// Obt�n el CheckBox que lanz� el evento
        //var checkBox = sender as CheckBox;

        //// Obt�n el objeto de producto a trav�s del BindingContext del CheckBox
        //var product = checkBox?.BindingContext as Products;

        //var vm = BindingContext as MainPageViewModel;

        //if (product != null && vm != null)
        //{
        //    await vm.UpdateProduct(product);
        //}
    }
}