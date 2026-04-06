using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WIS.Domain.Security
{
    public class Signer
    {
        public static byte[] ComputeHash(string secret, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(secret);
            using (var hmac = new HMACSHA512(bytes))
            {
                bytes = Encoding.UTF8.GetBytes(content);
                return hmac.ComputeHash(bytes);
            }
        }
    }
}
