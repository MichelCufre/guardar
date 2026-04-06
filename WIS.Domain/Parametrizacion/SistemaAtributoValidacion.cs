using System;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;

namespace WIS.Domain.Parametrizacion
{
    public class SistemaAtributoValidacion : IAtributoValidacionService
    {
        protected Atributo _atributo;
        protected ILpnServiceContext _serviceContext;

        public SistemaAtributoValidacion(Atributo atributo = null, ILpnServiceContext serviceContext = null)
        {
            _atributo = atributo;
            _serviceContext = serviceContext;
        }

        public virtual void Validar(IUnitOfWork uow, int idAtributo, short idValidacion, string valorAValidar, string valorComparativo, IFormatProvider culture, bool invocarAPICustom, out Error error)
        {
            error = null;
            switch (idValidacion)
            {
                case ValidacionAtributoDb.SistemaAPICustom:

                    if (invocarAPICustom)
                    {
                        ValidationAtributeAPI api = new ValidationAtributeAPI();
                        api.HttpPost(valorAValidar, valorComparativo, idAtributo, out error);
                    }

                    break;
            }
        }
    }
}
