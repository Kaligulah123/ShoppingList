

using ShoppingList.MVVM.ViewModels;

namespace ShoppingList.MVVM.Views
{
    public partial class MainPage : ContentPage
    {       
        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();

            BindingContext = vm;
        }

       
    }

}
