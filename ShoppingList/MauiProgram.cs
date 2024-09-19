using Firebase.Database;
using Microsoft.Extensions.Logging;
using ShoppingList.Data;
using ShoppingList.MVVM.ViewModels;
using ShoppingList.MVVM.Views;

namespace ShoppingList
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif         
            builder.Services.AddSingleton<DatabaseService>();

            builder.Services.AddSingleton<MainPage>()
                            .AddSingleton<MainPageViewModel>();

            builder.Services.AddSingleton<ProductsView>();
                           

            return builder.Build();           
        }

      
    }
}
