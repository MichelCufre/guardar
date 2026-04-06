using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WIS.Authentication
{
    public class JwtService : IAuthService
    {
        private readonly ISecretProvider _secretProvider;

        public JwtService(ISecretProvider secretProvider)
        {
            this._secretProvider = secretProvider;
        }

        public bool IsTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("No se recibio token de autorizacion");

            TokenValidationParameters tokens = this.GetTokenValidationParameters();

            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();

            try
            {
                ClaimsPrincipal tokenValid = jwtHandler.ValidateToken(token, tokens, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GenerateToken(IAuthContainerModel model)
        {
            if (model == null || !model.Claims.Any())
                throw new ArgumentException("No se recibieron parametros para crear token");

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(model.Claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(model.ExpireMinutes)),
                SigningCredentials = new SigningCredentials(this.GetSymmetricSecurityKey(), model.SecurityAlgorithm)
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);

            return jwtSecurityTokenHandler.WriteToken(securityToken);
        }

        public List<Claim> GetTokenClaims(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token es nulo");

            TokenValidationParameters tokenValidationParameters = this.GetTokenValidationParameters();

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            return tokenValid.Claims.ToList();
        }

        private SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symmetricKey = Convert.FromBase64String(this._secretProvider.GetSecret());

            return new SymmetricSecurityKey(symmetricKey);
        }

        public TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = this.GetSymmetricSecurityKey()
            };
        }
    }

}
