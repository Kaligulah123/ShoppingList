using ShoppingList.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingList.Selectors
{
    public class ShopListTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var shopList = item as ShopList;

            if (shopList == null) return new DataTemplate();

            if (shopList.IsShared)
            {                
                if (Application.Current.Resources.TryGetValue("IsSharedStyle", out var isSharedStyle) && isSharedStyle is DataTemplate template)
                {
                    return template; // Retorna el DataTemplate si se encontró y es válido.
                }
            }
            else
            {
                if (Application.Current.Resources.TryGetValue("NotSharedStyle", out var notSharedStyle) && notSharedStyle is DataTemplate template)
                {
                    return template; // Retorna el DataTemplate si se encontró y es válido.
                }
            }

            return new DataTemplate();

        }
    }
}
