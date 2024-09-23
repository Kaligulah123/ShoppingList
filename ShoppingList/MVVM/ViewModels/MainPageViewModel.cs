using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Database.Query;
using ShoppingList.Data;
using ShoppingList.Helpers;
using ShoppingList.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingList.MVVM.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        //Campos
        #region Campos

        private readonly DatabaseService _databaseService;
        public ObservableCollection<ShopList> ShopLists { get; set; } = new();
        public ObservableCollection<Products> ProductsList { get; set; } = new();

        [ObservableProperty]
        private ShopList? _selectedList;

        [ObservableProperty]
        private string? _listName;

        [ObservableProperty]
        private string? _productName;

        #endregion


        //Constructor
        #region Constructor

        public MainPageViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            //SubscribeShopLists();

            //SubscribeProductsLists();

            var currentUserId = Preferences.Get("UserId", null);

            SubscribeToCollection("ShopList", ShopLists, shopList => shopList.Name, true, currentUserId);

            SubscribeToCollection("Products", ProductsList, product => product.Name, false);
        }

        #endregion


        //Metodos
        #region Metodos             

        private void SubscribeToCollection<T>(string collectionName, ObservableCollection<T> observableCollection, Func<T, string> getKey, bool filterByUser = false, string? currentUserId = null) where T : class
        {
            _databaseService.Client.Child(collectionName)
                           .AsObservable<T>()
                           .Subscribe(dataSnapshot =>
                           {
                               var item = dataSnapshot.Object;

                               // Filtrar solo si es una ShopList y necesitamos aplicar el filtro por usuario
                               if (filterByUser && item is ShopList shopList)
                               {
                                   // Verificar si el usuario actual está en la lista de usuarios de la ShopList
                                   if (currentUserId == null || !shopList.Users.Contains(currentUserId))
                                   {
                                       return; // No agregar esta lista porque no pertenece al usuario actual
                                   }
                               }

                               switch (dataSnapshot.EventType)
                               {
                                   case Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate:
                                       if (item != null)
                                       {
                                           var existingItem = observableCollection.FirstOrDefault(i => getKey(i) == getKey(item));
                                           if (existingItem != null)
                                           {
                                               // Actualiza el item existente
                                               var index = observableCollection.IndexOf(existingItem);
                                               observableCollection[index] = item;
                                           }
                                           else
                                           {
                                               // Agrega el nuevo item
                                               observableCollection.Add(item);
                                           }
                                       }
                                       break;

                                   case Firebase.Database.Streaming.FirebaseEventType.Delete:
                                       var itemToRemove = observableCollection.FirstOrDefault(i => getKey(i) == getKey(item));
                                       if (itemToRemove != null)
                                       {
                                           observableCollection.Remove(itemToRemove);
                                       }
                                       break;
                               }
                           });
        }

        //private void SubscribeShopLists()
        //{
        //    _databaseService.Client.Child("ShopList")
        //                          .AsObservable<ShopList>()
        //                          .Subscribe(dataSnapshot =>
        //                          {
        //                              var shopList = dataSnapshot.Object;

        //                              switch (dataSnapshot.EventType)
        //                              {
        //                                  case Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate:

        //                                      if (shopList != null)
        //                                      {
        //                                          var existingItem = ShopLists.FirstOrDefault(item => item.Name == shopList.Name);

        //                                          if (existingItem != null)
        //                                          {
        //                                              // Actualiza el item existente
        //                                              var index = ShopLists.IndexOf(existingItem);

        //                                              ShopLists[index] = shopList;
        //                                          }
        //                                          else
        //                                          {
        //                                              ShopLists.Add(shopList);
        //                                          }
        //                                      }
        //                                      break;

        //                                  case Firebase.Database.Streaming.FirebaseEventType.Delete:

        //                                      var itemToRemove = ShopLists.FirstOrDefault(item => item.Name == shopList.Name);

        //                                      if (itemToRemove != null)
        //                                      {
        //                                          ShopLists.Remove(itemToRemove);
        //                                      }
        //                                      break;
        //                              }
        //                          });
        //}
        ////private void SubscribeProductsLists()
        ////{
        ////    // Obtener todas las ShopLists de Firebase
        ////    var shopListSnapshot = _databaseService.Client.Child("ShopList").OnceAsync<ShopList>();

        ////    // Convertir las ShopLists a un diccionario o lista para fácil búsqueda
        ////    var shopLists = shopListSnapshot.Result.Select(x => x.Object).ToList();

        ////    // Suscribirse a los cambios en la colección de Products en Firebase
        ////    _databaseService.Client.Child("Products")
        ////                          .AsObservable<Products>()
        ////                          .Subscribe(dataSnapshot =>
        ////                          {
        ////                              var product = dataSnapshot.Object;

        ////                              if (product == null) return;

        ////                              // Verificar si el producto está asociado a alguna ShopList por su ShopListName
        ////                              var matchingShopList = shopLists.FirstOrDefault(shopList => shopList.Name == product.ShopListName);

        ////                              if (matchingShopList == null) return;

        ////                              switch (dataSnapshot.EventType)
        ////                              {
        ////                                  case Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate:

        ////                                      // Verificar si el producto ya existe en la lista de productos
        ////                                      var existingProduct = ProductsList.FirstOrDefault(item => item.Name == product.Name);

        ////                                      if (existingProduct != null)
        ////                                      {
        ////                                          // Actualizar el producto existente
        ////                                          var index = ProductsList.IndexOf(existingProduct);

        ////                                          ProductsList[index] = product;
        ////                                      }
        ////                                      else
        ////                                      {
        ////                                          // Agregar el nuevo producto
        ////                                          ProductsList.Add(product);
        ////                                      }
        ////                                      break;

        ////                                  case Firebase.Database.Streaming.FirebaseEventType.Delete:
        ////                                      // Eliminar el producto de la lista si coincide con la ShopList
        ////                                      var productToRemove = ProductsList.FirstOrDefault(item => item.Name == product.Name);

        ////                                      if (productToRemove != null)
        ////                                      {
        ////                                          ProductsList.Remove(productToRemove);
        ////                                      }
        ////                                      break;
        ////                              }

        ////                          });
        ////}
        //private void SubscribeProductsLists()
        //{
        //    _databaseService.Client.Child("Products")
        //                          .AsObservable<Products>()
        //                          .Subscribe(dataSnapshot =>
        //                          {
        //                              var product = dataSnapshot.Object;

        //                              switch (dataSnapshot.EventType)
        //                              {
        //                                  case Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate:

        //                                      if (product != null)
        //                                      {
        //                                          var existingItem = ProductsList.FirstOrDefault(item => item.Name == product.Name);

        //                                          if (existingItem != null)
        //                                          {
        //                                              // Actualiza el item existente
        //                                              var index = ProductsList.IndexOf(existingItem);

        //                                              ProductsList[index] = product;
        //                                          }
        //                                          else
        //                                          {
        //                                              ProductsList.Add(product);
        //                                          }
        //                                      }
        //                                      break;

        //                                  case Firebase.Database.Streaming.FirebaseEventType.Delete:

        //                                      var itemToRemove = ProductsList.FirstOrDefault(item => item.Name == product.Name);

        //                                      if (itemToRemove != null)
        //                                      {
        //                                          ProductsList.Remove(itemToRemove);
        //                                      }
        //                                      break;
        //                              }
        //                          });
        //}
        public async Task GetProducts()
        {
            ProductsList.Clear();

            // Verificar que haya una lista seleccionada
            if (SelectedList == null || string.IsNullOrWhiteSpace(SelectedList.Name))
                return;

            // Obtener todos los productos de Firebase
            var productSnapshot = await _databaseService.Client.Child("Products").OnceAsync<Products>();

            // Filtrar productos que coincidan con el ShopListName de la lista seleccionada
            var matchingProducts = productSnapshot
                                    .Where(product => product.Object.ShopListName == SelectedList.Name)
                                    .Select(product => product.Object)
                                    .ToList();

            // Añadir los productos filtrados a la colección observable
            foreach (var product in matchingProducts)
            {
                ProductsList.Add(product);
            }
        }

        [RelayCommand]
        public async Task UpdateProduct(Products product)
        {
            if (product == null) return;

            var existingProduct = (await _databaseService.Client
                                .Child("Products")
                                .OnceAsync<Products>())
                                .FirstOrDefault(p => p.Object.Name == product.Name && p.Object.ShopListName == product.ShopListName);

            if (existingProduct != null)
            {
                // Actualiza el producto existente
                product.IsChecked = !product.IsChecked;

                await _databaseService.Client.Child($"Products/{existingProduct.Key}").PutAsync(product);
            }
        }

        #endregion


        //Comandos
        #region Comandos

        [RelayCommand]
        private async Task CreateList()
        {
            if (string.IsNullOrWhiteSpace(ListName)) return;

            var currentUser = Preferences.Get("UserId", null);

            if (string.IsNullOrWhiteSpace(currentUser)) return;

            var newShopList = new ShopList
            {
                Name = ListName,

                CreatedAt = DateTime.UtcNow,

                Code = MathHelper.GenerateRandomCode(),

                Users = new List<string> { currentUser }
            };

            var result = await _databaseService.Client.Child("ShopList").PostAsync(newShopList);

            ListName = string.Empty;

            //// Crea el enlace de WhatsApp
            //string message = $"Acabo de crear una nueva lista: {newShopList.Name}. El código es: {newShopList.Code}";
            //string phoneNumber = "+34699411368"; // Reemplaza con el número al que quieras enviar el mensaje
            //string whatsappUrl = $"https://wa.me/{phoneNumber}?text={Uri.EscapeDataString(message)}";

            //// Abre WhatsApp para enviar el mensaje
            //await Launcher.OpenAsync(whatsappUrl);          
        }


        [RelayCommand]
        private async Task ShareList(ShopList shopList)
        {
            //if (shopList == null) return;

            //// Crea el mensaje para compartir
            //string message = $"I,ve just created a new list: {shopList.Name}. Code: {shopList.Code}";

            //// Abre el diálogo de compartir
            //await Share.Default.RequestAsync(new ShareTextRequest
            //{
            //    Text = message,

            //    Title = "Share List"
            //});

            //// Solicita una confirmación al usuario
            //bool isShared = await Shell.Current.DisplayAlert("Share", "Did you share the list?", "Yes", "No");

            //if (isShared)
            //{
            //    // El usuario confirmó que compartió la lista
            //    await Shell.Current.DisplayAlert("Shared", "Thank you for sharing the list!", "OK");

            //    shopList.IsShared = true;

            //    // Encuentra el ID (Key) de la lista en Firebase
            //    var existingShopList = (await _databaseService.Client
            //                        .Child("ShopList")
            //                        .OnceAsync<ShopList>())
            //                        .FirstOrDefault(p => p.Object.Name == shopList.Name);

            //    if (existingShopList != null)
            //    {                  
            //        await _databaseService.Client.Child($"ShopList/{existingShopList.Key}").PutAsync(shopList);
            //    }
            //    else
            //    {
            //        // El usuario canceló la operación
            //        await Shell.Current.DisplayAlert("Cancelled", "It seems you didn't share the list.", "OK");
            //    }
            //}
            if (shopList == null) return;

            // Crea el mensaje para compartir con un enlace personalizado
            //string link = $"ShoppingList://shareList?code={shopList.Code}"; // Asegúrate de que "myapp" es el esquema que configuraste

            ////string link1 = "https://bit.ly/your-short-url";
            //string message = $"I've just created a new list: {shopList.Name}. Click here to view it: {link}";

            //string message1 = $"Lista creada: {shopList.Name}\n" +
            //        $"Código: {shopList.Code}\n" +
            //        $"Añade esta lista con este enlace: myapp://addlist?code={shopList.Code}";

            //string link2 = $"myapp://addlist?code={shopList.Code}"; // Asegúrate de que coincida con tu configuración

            // Abre el diálogo de compartir
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                //Text = message,
                Text = $"{shopList.Code}",
                Title = "Share List"
            });

            shopList.IsShared = true;

            // Solicita una confirmación al usuario
            bool isShared = await Shell.Current.DisplayAlert("Share", "Did you share the list?", "Yes", "No");

            if (isShared)
            {
                // El usuario confirmó que compartió la lista
                await Shell.Current.DisplayAlert("Shared", "Thank you for sharing the list!", "OK");                

                // Encuentra el ID (Key) de la lista en Firebase
                var existingShopList = (await _databaseService.Client
                                    .Child("ShopList")
                                    .OnceAsync<ShopList>())
                                    .FirstOrDefault(p => p.Object.Name == shopList.Name);

                if (existingShopList != null)
                {
                    await _databaseService.Client.Child($"ShopList/{existingShopList.Key}").PutAsync(shopList);
                }
            }
            else
            {
                shopList.IsShared = false;

                // El usuario canceló la operación
                await Shell.Current.DisplayAlert("Cancelled", "It seems you didn't share the list.", "OK");
            }

            //// Crea el enlace personalizado
            //string link = $"myapp://addlist?code={shopList.Code}"; // Asegúrate de que "myapp" es el esquema que configuraste

            //// Crea el mensaje para compartir
            //string message = $"He creado una nueva lista: {shopList.Name}. " +
            //                 $"Añade esta lista con este enlace: {link}";

            //// Escapa el mensaje para asegurarte de que sea seguro para URLs
            //string whatsappUrl = $"https://wa.me/?text={Uri.EscapeDataString(message)}";

            //// Abre el enlace de WhatsApp en el navegador
            //await Browser.OpenAsync(whatsappUrl);
        }


        [RelayCommand]
        private async Task DeleteList(ShopList shopList)
        {
            SelectedList = shopList;

            var answer = await Shell.Current.DisplayAlert("ATENTION!", $"Do you really want to delete {SelectedList.Name} from Lists?", "YES", "NO");

            if (answer)
            {
                var shopListToDelete = (await _databaseService.Client.Child("ShopList")
                    .OnceAsync<ShopList>())
                    .FirstOrDefault(x => x.Object.Name == shopList.Name);

                if (shopListToDelete != null)
                {
                    // Encuentra el UserId del usuario actual
                    string userId = Preferences.Get("UserId", string.Empty); // Cambia "UserId" por la clave correcta en tus preferencias

                    // Si es el único usuario, elimina la lista completa
                    if (shopListToDelete.Object.Users.Count == 1 && shopListToDelete.Object.Users.Contains(userId))
                    {
                        // Primero, eliminar los productos asociados a la ShopList
                        var productsToDelete = (await _databaseService.Client.Child("Products")
                            .OnceAsync<Products>())
                            .Where(p => p.Object.ShopListName == shopList.Name)
                            .ToList();

                        // Eliminar cada producto asociado
                        foreach (var product in productsToDelete)
                        {
                            await _databaseService.Client.Child("Products").Child(product.Key).DeleteAsync();
                        }

                        // Luego, eliminar la ShopList
                        await _databaseService.Client.Child("ShopList").Child(shopListToDelete.Key).DeleteAsync();
                    }
                    else if (shopListToDelete.Object.Users.Contains(userId))
                    {
                        // Eliminar solo al usuario de la lista
                        shopListToDelete.Object.Users.Remove(userId);

                        // Actualizar la lista en Firebase
                        await _databaseService.Client.Child($"ShopList/{shopListToDelete.Key}")
                            .PutAsync(shopListToDelete.Object);

                        // Asegúrate de que la colección también se actualice
                        var existingItem = ShopLists.FirstOrDefault(i => i.Name == shopListToDelete.Object.Name);
                        if (existingItem != null)
                        {
                            // Si no quedan usuarios, elimina la lista de la colección
                            ShopLists.Remove(existingItem);
                        }
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "You do not have permission to delete this list.", "OK");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "List not found.", "OK");
                }
            }
        }

        [RelayCommand]
        private async Task AddProduct()
        {
            if (string.IsNullOrWhiteSpace(ProductName) || SelectedList?.Name == null) return;

            var product = new Products
            {
                Name = ProductName,

                ShopListName = SelectedList?.Name
            };

            var result = await _databaseService.Client.Child("Products").PostAsync(product);

            ProductName = string.Empty;
        }

        [RelayCommand]
        private async Task DeleteProduct(Products product)
        {
            if (product != null)
            {               
                var answer = await Shell.Current.DisplayAlert("ATENTION!", $"Do you really want to delete {product.Name} from Lists?", "YES", "NO");

                if (answer)
                {
                    var productToDelete = (await _databaseService.Client.Child("Products")
                        .OnceAsync<Products>())
                        .FirstOrDefault(x => x.Object.Name == product.Name);

                    if (productToDelete != null)
                    {
                        // Elimina el nodo con la clave correspondiente
                        await _databaseService.Client.Child("Products").Child(productToDelete.Key).DeleteAsync();
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Product not found.", "OK");
                    }
                }
            }
        }


        [RelayCommand]
        private async Task OpenList(ShopList shopList)
        {
            if (shopList != null)
            {
                SelectedList = shopList;

                await Shell.Current.GoToAsync("ProductsView");
            }
        }


        [RelayCommand]
        private async void AddListFromCode()
        {
           var listCode = await Shell.Current.DisplayPromptAsync("Code", "Enter shared Code", "Ok", "Cancel");

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
            else
            {
                await Shell.Current.DisplayAlert("Error", "Code is not correct", "OK");
            }
        }

        #endregion

    }
}
