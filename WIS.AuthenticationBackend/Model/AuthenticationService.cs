using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WIS.AuthenticationBackend.Model
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _uow;

        public AuthenticationService(IUnitOfWork uow)
        {
            this._uow = uow;
        }

        public async Task<AuthenticationResponse> Authenticate(AuthenticationRequest userData)
        {
            User user = await this._uow.UserRepository.GetByUsername(userData.Username);

            if (user is null)
                throw new InvalidOperationException("User not found");

            if (!user.IsEnabled)
                throw new InvalidOperationException("User is not enabled");

            if (user.IsLocked)
                throw new InvalidOperationException("User is locked");

            if (user.Password != this.EncodePasswordLegacy(userData.Password, PasswordFormats.Hashed, user.Salt))
                throw new InvalidOperationException("Invalid password");

            return new AuthenticationResponse(user);
        }

        //Eliminenme por favoooor
        private string EncodePasswordLegacy(string password, PasswordFormats passwordFormat, string encodedSalt)
        {
            if (passwordFormat == PasswordFormats.Clear) // Clear
                return password;

            byte[] PASSWORDBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Convert.FromBase64String(encodedSalt);
            byte[] combinedBytes = new byte[saltBytes.Length + PASSWORDBytes.Length];
            byte[] cryptedBytes = combinedBytes;

            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(PASSWORDBytes, 0, combinedBytes, saltBytes.Length, PASSWORDBytes.Length);

            if (passwordFormat == PasswordFormats.Hashed)
            {
                HashAlgorithm hashAlgorithm = HashAlgorithm.Create("SHA1"); //AAAAAHHHHHHH
                cryptedBytes = hashAlgorithm.ComputeHash(combinedBytes);

                /*using (SHA256 sha256Hash = SHA256.Create())
                {
                    // ComputeHash SHA256 - Mas seguro que SHA1
                    cryptedBytes = sha256Hash.ComputeHash(combinedBytes);
                }*/
            }

            return Convert.ToBase64String(cryptedBytes);
        }
    }
}
