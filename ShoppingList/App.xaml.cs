using Firebase.Database.Query;
using ShoppingList.Data;
using ShoppingList.MVVM.Models;
using ShoppingList.MVVM.Views;
using ShoppingList.Services;

namespace ShoppingList
{
    public partial class App : Application
    {
        private readonly DatabaseService _databaseService;

        public App(DatabaseService databaseService)
        {
            InitializeComponent();

            RegisterRoutes();

            MainPage = new AppShell();

            _databaseService = databaseService;
        }
        private void RegisterRoutes()
        {
            Routing.RegisterRoute("ProductsView", typeof(ProductsView));
            Routing.RegisterRoute("RegisterView", typeof(RegisterView));
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);

            // Verifica que el esquema sea "myapp" y el host "addlist"
            if (uri.Scheme == "myapp" && uri.Host == "addlist")
            {
                // Extrae el código de la lista del URI, por ejemplo: myapp://addlist?code=ABC123
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

                string? listCode = query.Get("code");

                if (!string.IsNullOrEmpty(listCode))
                {
                    // Aquí llamas a la función para añadir la lista con el código
                    AddListFromCode(listCode);
                }
            }
        }

        private async void AddListFromCode(string listCode)
        {
            //var shopListSnapshot = await _databaseService.Client.Child("ShopList").OnceAsync<ShopList>();

            //// Filtrar productos que coincidan con el ShopListName de la lista seleccionada
            //var matchingList = shopListSnapshot
            //                        .Where(shoplist => shoplist.Object.Code == listCode)
            //                        .Select(shoplist => shoplist.Object)
            //                        .ToList();

            //// Añadir los productos filtrados a la colección observable
            //foreach (var product in matchingProducts)
            //{
            //    ProductsList.Add(product);
            //}

            //if (listCode == null) return;

            //var existingShopList = (await _databaseService.Client
            //                    .Child("ShopList")
            //                    .OnceAsync<ShopList>())
            //                    .FirstOrDefault(p => p.Object.Code == listCode);

            //if (existingShopList != null)
            //{
            //    // Actualiza el producto existente
            //    existingShopList.Users.Add("ss");

            //    await _databaseService.Client.Child($"Products/{existingProduct.Key}").PutAsync(product);
            //}
            if (string.IsNullOrEmpty(listCode)) return;

            // Obtén todas las listas de compras de Firebase
            var shopListSnapshot = await _databaseService.Client
                .Child("ShopList")
                .OnceAsync<ShopList>();

            // Encuentra la lista existente con el código proporcionado
            var existingShopList = shopListSnapshot
                .FirstOrDefault(p => p.Object.Code == listCode);

            if (existingShopList != null)
            {
                // Accede al objeto ShopList
                var shopList = existingShopList.Object;

                // Añadir el UserId a la lista de usuarios
                string userId = Preferences.Get("UserId", string.Empty); // Cambia "UserId" por la clave correcta en tus preferencias
                if (!string.IsNullOrEmpty(userId) && !shopList.Users.Contains(userId))
                {
                    shopList.Users.Add(userId);
                }

                // Actualiza la lista en Firebase
                await _databaseService.Client
                    .Child($"ShopList/{existingShopList.Key}")
                    .PutAsync(shopList);
            }
        }
    }
    
}
