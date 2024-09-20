using Firebase.Auth.Providers;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingList.Services
{
    public class FireBaseAuthService
    {
        public static FirebaseAuthClient ConnectToFirebase()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyDIaAD63l3nLcCwdb3JzM8ZDdMeYYxBCRU",

                AuthDomain = "shoppinglist-5f074.web.app",

                Providers = new FirebaseAuthProvider[]

            {
                // Add and configure individual providers

                new GoogleProvider().AddScopes("email"),

                new EmailProvider()

                // ...
            },
            };

            var client = new FirebaseAuthClient(config);

            return client;
        }

        public async Task<UserCredential> LoginUser(string email, string password)
        {
            var client = ConnectToFirebase();

            var userCredential = await client.SignInWithEmailAndPasswordAsync(email, password);

            return userCredential;
        }

        public async Task<UserCredential> RegisterUser(string email, string password)
        {
            var client = ConnectToFirebase();

            var userCredential = await client.CreateUserWithEmailAndPasswordAsync(email, password);

            return userCredential;
        }
    }
}
