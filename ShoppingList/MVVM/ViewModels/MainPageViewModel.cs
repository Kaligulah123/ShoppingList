using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Database.Query;
using ShoppingList.Data;
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

            SubscribeShopLists();

            SubscribeProductsLists();

        }

        #endregion


        //Metodos
        #region Metodos             

        private void SubscribeShopLists()
        {            
            _databaseService.Client.Child("ShopList")
                                  .AsObservable<ShopList>()
                                  .Subscribe(dataSnapshot =>
                                  {
                                      var shopList = dataSnapshot.Object;

                                      switch (dataSnapshot.EventType)
                                      {
                                          case Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate:                                            
                                              
                                              if (shopList != null)
                                              {
                                                  var existingItem = ShopLists.FirstOrDefault(item => item.Name == shopList.Name);

                                                  if (existingItem != null)
                                                  {
                                                      // Actualiza el item existente
                                                      var index = ShopLists.IndexOf(existingItem);

                                                      ShopLists[index] = shopList;
                                                  }
                                                  else
                                                  {                                                      
                                                      ShopLists.Add(shopList);
                                                  }
                                              }
                                              break;

                                          case Firebase.Database.Streaming.FirebaseEventType.Delete:                                            

                                              var itemToRemove = ShopLists.FirstOrDefault(item => item.Name == shopList.Name);

                                              if (itemToRemove != null)
                                              {
                                                  ShopLists.Remove(itemToRemove);
                                              }
                                              break;
                                      }
                                  });
        }
        private void SubscribeProductsLists()
        {
            // Obtener todas las ShopLists de Firebase
            var shopListSnapshot = _databaseService.Client.Child("ShopList").OnceAsync<ShopList>();

            // Convertir las ShopLists a un diccionario o lista para fácil búsqueda
            var shopLists = shopListSnapshot.Result.Select(x => x.Object).ToList();

            // Suscribirse a los cambios en la colección de Products en Firebase
            _databaseService.Client.Child("Products")
                                  .AsObservable<Products>()
                                  .Subscribe(dataSnapshot =>
                                  {
                                  var product = dataSnapshot.Object;

                                  if (product == null) return;

                                  // Verificar si el producto está asociado a alguna ShopList por su ShopListName
                                  var matchingShopList = shopLists.FirstOrDefault(shopList => shopList.Name == product.ShopListName);

                                  if (matchingShopList == null) return;

                                      switch (dataSnapshot.EventType)
                                      {
                                          case Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate:

                                              // Verificar si el producto ya existe en la lista de productos
                                              var existingProduct = ProductsList.FirstOrDefault(item => item.Name == product.Name);

                                              if (existingProduct != null)
                                              {
                                                  // Actualizar el producto existente
                                                  var index = ProductsList.IndexOf(existingProduct);

                                                  ProductsList[index] = product;
                                              }
                                              else
                                              {
                                                  // Agregar el nuevo producto
                                                  ProductsList.Add(product);
                                              }
                                              break;

                                          case Firebase.Database.Streaming.FirebaseEventType.Delete:
                                              // Eliminar el producto de la lista si coincide con la ShopList
                                              var productToRemove = ProductsList.FirstOrDefault(item => item.Name == product.Name);

                                              if (productToRemove != null)
                                              {
                                                  ProductsList.Remove(productToRemove);
                                              }
                                              break;
                                      }

                                  });
        }

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

        #endregion


        //Comandos
        #region Comandos

        [RelayCommand]
        private async Task CreateList()
        {           
            if (string.IsNullOrWhiteSpace(ListName)) return;

            var newShopList = new ShopList
            {
                Name = ListName,

                CreatedAt = DateTime.UtcNow
            };

            var result = await _databaseService.Client.Child("ShopList").PostAsync(newShopList);                     

            ListName = string.Empty;
        }


        [RelayCommand]
        private async Task DeleteList(ShopList shopList)
        {
            if (shopList != null)
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
                        // Elimina el nodo con la clave correspondiente
                        await _databaseService.Client.Child("ShopList").Child(shopListToDelete.Key).DeleteAsync();
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "List not found.", "OK");
                    }
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
        private async Task OpenList(ShopList shopList)
        {
            if (shopList != null)
            {
                SelectedList = shopList;

                await Shell.Current.GoToAsync("ProductsView");
            }               
        }

        #endregion

    }
}
