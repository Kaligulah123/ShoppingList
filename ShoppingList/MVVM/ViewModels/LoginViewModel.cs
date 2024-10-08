﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth.Providers;
using Newtonsoft.Json;
using ShoppingList.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingList.MVVM.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {

        //Campos
        #region Campos

        [ObservableProperty]
        public string? _email;

        [ObservableProperty]
        public string? _password;

        [ObservableProperty]
        private bool _isEntriesNotEnabled = true;


        #endregion


        //Constructor
        #region Constructor      



        #endregion


        //Metodos
        #region Metodos
        public void ClearCredentials()
        {
            Email = string.Empty;
            Password = string.Empty;
        }

        #endregion


        //Comandos
        #region Comandos

        [RelayCommand]
        private async Task LoginUser()
        {
            if (string.IsNullOrWhiteSpace(Email) || (string.IsNullOrWhiteSpace(Password))) return;

            try
            {
                IsEntriesNotEnabled = false;

                FireBaseAuthService fireBaseAuthService = new();

                var credentials = await fireBaseAuthService.LoginUser(Email, Password);

                var Uid = credentials.User.Uid;

                Preferences.Set("UserId", Uid);                            

                await Shell.Current.GoToAsync("//MainPage");

                IsEntriesNotEnabled = true;
            }
            catch (Exception ex)
            {               
                await Shell.Current.DisplayAlert("ALERT!", ex.Message, "OK");

                IsEntriesNotEnabled = true;
            }
        }


        [RelayCommand]
        private async Task RegisterUser()
        {
            if (string.IsNullOrWhiteSpace(Email) || (string.IsNullOrWhiteSpace(Password))) return;

            try
            {
                IsEntriesNotEnabled = false;

                FireBaseAuthService fireBaseAuthService = new();

                var credentials = await fireBaseAuthService.RegisterUser(Email, Password);

                var Uid = credentials.User.Uid;

                if (Uid != null)
                {
                    await Shell.Current.DisplayAlert("Alert", "User Registered successfully!", "OK");                   

                    await Shell.Current.GoToAsync("//LoginView");

                    IsEntriesNotEnabled = true;
                }
            }
            catch (Exception ex)
            {               
                await Shell.Current.DisplayAlert("ALERT!", ex.Message, "OK");

                IsEntriesNotEnabled = true;
            }
        }

        [RelayCommand]
        private async Task GoToRegisterView()
        {
            await Shell.Current.GoToAsync("RegisterView");
        }

        #endregion

    }
}

