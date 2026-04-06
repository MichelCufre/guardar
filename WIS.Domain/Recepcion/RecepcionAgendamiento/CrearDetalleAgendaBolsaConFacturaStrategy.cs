using System.Collections.Generic;
using WIS.Domain.DataModel;

namespace WIS.Domain.Recepcion.RecepcionAgendamiento
{
    public class CrearDetalleAgendaBolsaConFacturaStrategy : ICrearDetallesAgendaStrategy
    {
        protected IUnitOfWork _uow;
        protected List<int> _referencias;
        protected List<int> _facturas;

        public CrearDetalleAgendaBolsaConFacturaStrategy(IUnitOfWork uow, List<int> keyReferencias, List<int> keyFacturas)
        {
            _uow = uow;
            _referencias = keyReferencias;
            _facturas = keyFacturas;
        }

        public virtual List<AgendaDetalle> CrearDetallesAgenda()
        {
            return new List<AgendaDetalle>();
        }
    }
}
