using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingList.Data
{
    public class DatabaseService
    {
        public FirebaseClient Client { get; private set; }
        public DatabaseService()
        {
            Client = new FirebaseClient("https://shoppinglist-5f074-default-rtdb.europe-west1.firebasedatabase.app/");
        }
    }
}
