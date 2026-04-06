using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.Picking;
using WIS.Filtering;
using WIS.GridComponent.Execution.Configuration;

namespace WIS.Domain.Recepcion
{
    public interface ICrossDocking
    {
        public int Agenda { get; set; }
        public int Preparacion { get; set; }
        public int Usuario { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string Estado { get; set; }
        public string Tipo { get; set; }
        public List<LineaCrossDocking> Lineas { get; set; }

        public void AddPedidos(IUnitOfWork uow, IEnumerable<Pedido> pedidos);
        public void RemovePedidos(IUnitOfWork uow, List<Pedido> pedidos);
        public void AddPreparacion(IUnitOfWork uow, int empresa, string predio);        
        public void RemovePreparacion(IUnitOfWork uow);
        public bool CanEdit();
        public static abstract bool PuedeCrearCrossDock(IUnitOfWork uow, Agenda agenda);
        public bool PuedeFinalizarCrossDock();
        public void FinalizarCrossDocking(IUnitOfWork uow, int numeroAgenda, int empresa);
        public void Liberar(IUnitOfWork uow, Agenda agenda, List<Carga> cargas);
        public void Iniciar(IUnitOfWork uow, Agenda agenda, bool consumirOtrosDocumentos);
        public IEnumerable<Pedido> GetPedidosToAdd(IUnitOfWork uow, Agenda agenda, CrossDockingSeleccionTipo tipoSeleccion, GridMenuItemActionContext context, List<string> gridKeys, IFilterInterpreter filterInterpreter);
    }
}
