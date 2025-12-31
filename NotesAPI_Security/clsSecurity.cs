
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NotesAPI_Security
{
    public class clsSecurity
    {

        public static string GenerateSalt(int Size = 16)
        {
            byte[] ByteSalt = new byte[Size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(ByteSalt);
            }
            return Convert.ToBase64String(ByteSalt);
        }
        public static string Has256Password(string Password, string Salt)
        {


            using (SHA256 sha256 = SHA256.Create())
            {
                string PasswordSalt = Password + Salt;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(PasswordSalt));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        public static string Hash256Password(string Password)
        {


            using (SHA256 sha256 = SHA256.Create())
            {

                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

    }
}
