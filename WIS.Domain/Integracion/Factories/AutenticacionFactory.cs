using WIS.Domain.Integracion.Authentication;
using WIS.Domain.Integracion.Enums;
using WIS.Domain.Security;

namespace WIS.Domain.Integracion.Factories
{
    public class AutenticacionFactory
    {
        public IAuthenticationMethod Create(IntegracionServicio integration)
        {
            switch (integration.TipoAutenticacion)
            {
                case TipoAutenticacionDb.BASIC: return new AuthenticationBasic(integration.User, SecurityLogic.GetSecret(integration));
                case TipoAutenticacionDb.SECRET: return new AutheticationSecret(SecurityLogic.GetSecret(integration));
                case TipoAutenticacionDb.OAUTH2: return new AuthenticationOAuth2(integration.User, SecurityLogic.GetSecret(integration), integration.Scope, integration.UrlAuthServer);
            }

            return null;
        }
    }
}
