using WIS.Domain.Recepcion.RecepcionAgendamiento;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.Recepcion;
using WIS.Security;

namespace WIS.Domain.Recepcion
{
    public class DesarmadoAsociacion
    {
        protected readonly IUnitOfWorkFactory _uow;
        protected readonly IFactoryService _factoryService;
        protected readonly IIdentityService _identity;
        protected readonly List<AsociarAgenda> _asociarAgenda;


        public DesarmadoAsociacion(IUnitOfWorkFactory uow,
            IFactoryService factoryService,
            IIdentityService identity,
            List<AsociarAgenda> asociarAgenda)
        {
            this._uow = uow;
            this._factoryService = factoryService;
            this._identity = identity;
            this._asociarAgenda = asociarAgenda;
        }

        public virtual void Desarmar()
        {
            using var uow = this._uow.GetUnitOfWork();
            
            foreach (var item in this._asociarAgenda)
            {
                this.DesasociarAgendas(item);
            }

            uow.SaveChanges();
        }

        public virtual void DesasociarAgendas(AsociarAgenda asociarAgenda)
        {
            using var uow = this._uow.GetUnitOfWork();

            foreach (var linea in this._asociarAgenda)
            {
                Factura factura = uow.FacturaRepository.GetFactura(linea.IdFactura, false);
                factura.Agenda = null;
                uow.FacturaRepository.AddUpdateIdAgendaFactura(factura);
            }

            uow.SaveChanges();
        }
    }
}
