using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Security.Enums;
using WIS.Exceptions;
using WIS.Validation;

namespace WIS.Domain.Security
{
    /// <summary>
    /// La clase tiene por finalidad
    /// + Realizar el encode del password
    /// </summary>
    public class SeguridadEncodePassword
    {

        #region  ENCODE-PASSWORD

        public virtual string[] EncodePASSWORD(string password, FormatoContrasenia passwordFormat, string encodedSalt)
        {
            string[] retornoPassword;

            if (passwordFormat == FormatoContrasenia.Clear)
                return new string[] { password, encodedSalt };

            byte[] PASSWORDBytes = Encoding.UTF8.GetBytes(password);
            if (string.IsNullOrEmpty(encodedSalt))
                encodedSalt = GenerateEncodedRandomSalt();

            byte[] saltBytes = Convert.FromBase64String(encodedSalt);
            byte[] combinedBytes = new byte[saltBytes.Length + PASSWORDBytes.Length];
            byte[] cryptedBytes = null;

            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(PASSWORDBytes, 0, combinedBytes, saltBytes.Length, PASSWORDBytes.Length);

            if (passwordFormat == FormatoContrasenia.Hashed)
            {
                HashAlgorithm hashAlgorithm = HashAlgorithm.Create("SHA1");
                cryptedBytes = hashAlgorithm.ComputeHash(combinedBytes);

                /*using (SHA256 sha256Hash = SHA256.Create())
                {
                    // ComputeHash SHA256 - Mas seguro que SHA1
                    cryptedBytes = sha256Hash.ComputeHash(combinedBytes);
                }*/
            }
            else
            {
                cryptedBytes = EncryptPASSWORD(combinedBytes);
            }
            return new string[] { Convert.ToBase64String(cryptedBytes), encodedSalt };
        }

        public static string GenerateRandomSalt()
        {
            //  TODO: true random algorithm
            return Guid.NewGuid().ToString();
        }
        public virtual string GenerateEncodedRandomSalt()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(GenerateRandomSalt()));
        }

        // TODO: sin implementar
        public static byte[] EncryptPASSWORD(byte[] rawBytes)
        {
            return rawBytes;
        }
        public static byte[] DecryptPASSWORD(byte[] cryptedBytes)
        {
            return cryptedBytes;
        }
        #endregion

    }
}
