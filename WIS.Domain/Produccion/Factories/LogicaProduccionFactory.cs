using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Models;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Logic;
using WIS.Domain.DataModel;
using WIS.Security;
using WIS.Exceptions;

namespace WIS.Domain.Produccion.Factories
{
    public class LogicaProduccionFactory : ILogicaProduccionFactory
    {
        protected readonly IIngresoProduccionFactory _ingresoProduccionFactory;
        protected readonly IIdentityService _identity;

        public LogicaProduccionFactory(IIdentityService identity, IIngresoProduccionFactory ingresoProduccionFactory)
        {
            this._identity = identity;
            this._ingresoProduccionFactory = ingresoProduccionFactory;
        }

        public virtual ILogicaProduccion CreateLogicaProduccion(IUnitOfWork uow, string type)
        {
            IngresoProduccion ingresoProduccion = _ingresoProduccionFactory.CreateIngresoProduccion(type);
            return this.CreateLogicaProduccion(uow, type, ingresoProduccion);
        }

        public virtual ILogicaProduccion GetLogicaProduccion(IUnitOfWork uow, string nuIngresoProduccion)
        {
            IngresoProduccion ingresoProduccion = uow.IngresoProduccionRepository.GetIngresoByIdConDetalles(nuIngresoProduccion);
            return this.CreateLogicaProduccion(uow, ingresoProduccion.Tipo, ingresoProduccion);
        }

        public virtual ILogicaProduccion GetLogicaProduccion(IUnitOfWork uow, string idExterno, int empresa)
        {
            IngresoProduccion ingresoProduccion = uow.IngresoProduccionRepository.GetIngresoByIdExternoEmpresaConDetalles(idExterno, empresa);

            if (ingresoProduccion == null)
                throw new ValidationFailedException("General_Sec0_Error_Error98", new string[] { idExterno, empresa.ToString() });

            return this.CreateLogicaProduccion(uow, ingresoProduccion.Tipo, ingresoProduccion);
        }

        public virtual ILogicaProduccion CreateLogicaProduccion(IUnitOfWork uow, string type, IngresoProduccion ingreso)
        {
            switch (type)
            {
                case TipoIngresoProduccion.BlackBox:
                    return new LogicaProduccionBlackBox(uow, _identity, (IngresoProduccionBlackBox)ingreso);
                case TipoIngresoProduccion.WhiteBox:
                    return new LogicaProduccionWhiteBox(uow, _identity, (IngresoProduccionWhiteBox)ingreso);
                case TipoIngresoProduccion.Colector:
                    return new LogicaProduccionColector(uow, _identity, (IngresoProduccionColector)ingreso);
                default:
                    return new LogicaProduccionBlackBox(uow, _identity, (IngresoProduccionBlackBox)ingreso);
            }
        }

    }
}
