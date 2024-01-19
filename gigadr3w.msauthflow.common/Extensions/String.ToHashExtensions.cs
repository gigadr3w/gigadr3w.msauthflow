using System.Security.Cryptography;
using System.Text;

namespace gigadr3w.msauthflow.common.Extensions
{
    public static class StringToHashExtensions
    {
        public static string Hash256(this string s) 
        {
            string hash = string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));
                for (int i = 0; i < hashValue.Length; i++)
                {
                    hash += $"{hashValue[i]:X2}";
                }
            }

            return hash;
        }
    }
}
