using ShoppingList.MVVM.ViewModels;

namespace ShoppingList.MVVM.Views;

public partial class ProductsView : ContentPage
{
	public ProductsView(MainPageViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}