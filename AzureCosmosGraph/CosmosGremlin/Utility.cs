using System;
using System.Security.Cryptography;
using System.Text;

namespace CosmosGremlin
{
    internal class Utility
    {
        public static Guid CreateGuid(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                return new Guid(hash);
            }
        }
    }
}