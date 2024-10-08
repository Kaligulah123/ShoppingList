﻿using Firebase.Database;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
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

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainPageViewModel>();

            builder.Services.AddTransient<LoginView>();
            builder.Services.AddTransient<LoginViewModel>();

            builder.Services.AddTransient<RegisterView>();

            builder.Services.AddTransient<ProductsView>();           

            return builder.Build();           
        }

      
    }
}
