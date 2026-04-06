using WIS.Domain.Recepcion.RecepcionAgendamiento;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Security;

namespace WIS.Domain.Recepcion
{
    public class ArmadoAsociarAgendaFactura : IArmadoAsociarAgendaFactura
    {
        protected readonly int _idAgenda;
        protected readonly List<AsociarAgenda> _asociarAgenda;
        protected readonly IUnitOfWork _uow;
        protected readonly IFactoryService _factoryService;
        protected readonly IIdentityService _identity;

        public ArmadoAsociarAgendaFactura(IUnitOfWork uow,
            IFactoryService factoryService,
            IIdentityService identity,
            int idAgenda,
            List<AsociarAgenda> asociarAgenda)
        {
            this._uow = uow;
            this._factoryService = factoryService;
            this._identity = identity;
            this._asociarAgenda = asociarAgenda;
            this._idAgenda = idAgenda;
        }

        public virtual void Armar()
        {
            foreach (var agenda in this._asociarAgenda)
            {
                this.AgregarLinea(agenda);
            }

            _uow.SaveChanges();
        }

        public virtual void AgregarLinea(AsociarAgenda unidad)
        {
            Factura factura = _uow.FacturaRepository.GetFactura(unidad.IdFactura,false);

            if (factura.Situacion == SituacionDb.Activo)
            {
                factura.Agenda = this._idAgenda;
                _uow.FacturaRepository.AddUpdateIdAgendaFactura(factura);
            }
        }
    }
}
