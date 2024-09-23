using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingList.Helpers
{
    public static class MathHelper
    {
        public static string GenerateRandomCode()
        {
            Random random = new Random();

            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            const string digits = "0123456789";

            string randomLetters = new string(Enumerable.Repeat(letters, 3)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            string randomDigits = new string(Enumerable.Repeat(digits, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return randomLetters + randomDigits;
        }
    }
}
