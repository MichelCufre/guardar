using System.Security.Cryptography;
using WIS.Domain.DataModel;
using WIS.Domain.Integracion;
using WIS.Domain.Integracion.Constants;
using WIS.Domain.Integracion.Enums;

namespace WIS.Domain.Security
{
    public class SecurityLogic
    {
        public static string GetSecret(IntegracionServicio integration)
        {
            if (!string.IsNullOrEmpty(integration.Secret))
            {
                if (string.IsNullOrEmpty(integration.SecretSalt))
                {
                    return integration.Secret;
                }

                return Encrypter.Decrypt(integration.Secret, integration.SecretSalt, (int)integration.SecretFormat);
            }

            return null;
        }

        public static bool IsValidUser(IUnitOfWork uow, string loginName, byte[] hash)
        {
            var integracion = uow.IntegracionServicioRepository.GetIntegrationByCodigo(IntegracionesDb.AutomationInterpreter);

            if (integracion != null
                && integracion.TipoAutenticacion == TipoAutenticacionDb.SECRET
                && !string.IsNullOrEmpty(integracion.Secret))
            {
                var secret = GetSecret(integracion);
                var computedHash = Signer.ComputeHash(secret, loginName);

                if (CryptographicOperations.FixedTimeEquals(computedHash, hash))
                {
                    var user = uow.SecurityRepository.GetUser(loginName);
                    return user?.IsEnabled ?? false;
                }
            }

            return false;
        }

        public static string GetSecret(IUnitOfWork uow)
        {
            var integracion = uow.IntegracionServicioRepository.GetIntegrationByCodigo(IntegracionesDb.AutomationInterpreter);

            if (integracion != null
                && integracion.TipoAutenticacion == TipoAutenticacionDb.SECRET
                && !string.IsNullOrEmpty(integracion.Secret))
            {
                return GetSecret(integracion);
            }

            return "";
        }
    }
}
