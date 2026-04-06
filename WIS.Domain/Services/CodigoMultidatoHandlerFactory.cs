using System;
using System.Collections.Generic;
using WIS.Domain.CodigoMultidato;
using WIS.Domain.CodigoMultidato.Constants;
using WIS.Domain.CodigoMultidato.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class CodigoMultidatoHandlerFactory : ICodigoMultidatoHandlerFactory
    {
        public virtual ICodigoMultidatoHandler GetHandler(IUnitOfWork uow, string cdCodigoMultidato, string vlCodigoMultidato, out Dictionary<string, string> ais)
        {
            var handler = (ICodigoMultidatoHandler)null;
            ais = null;

            if (!string.IsNullOrEmpty(cdCodigoMultidato))
            {
                handler = GetHandler(cdCodigoMultidato);
                ais = handler.GetAIs(uow, vlCodigoMultidato);
                return handler;
            }

            handler = new EAN128Handler();
            if (handler.IsValid(uow, vlCodigoMultidato, out ais))
                return handler;

            return null;
        }

        public virtual ICodigoMultidatoHandler GetHandler(string cdCodigoMultidato)
        {
            switch (cdCodigoMultidato)
            {
                case TipoCodigoMultidato.EAN128:
                    return new EAN128Handler();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
