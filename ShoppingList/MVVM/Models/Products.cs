using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingList.MVVM.Models
{
    public partial class Products : ObservableObject
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? ShopListName { get; set; }

        [ObservableProperty]
        public bool isChecked;

    }
}
