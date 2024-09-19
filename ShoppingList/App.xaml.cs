using ShoppingList.MVVM.Views;

namespace ShoppingList
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            RegisterRoutes();

            MainPage = new AppShell();
        }
        private void RegisterRoutes()
        {
            Routing.RegisterRoute("ProductsView", typeof(ProductsView));          

        }
    }
}
