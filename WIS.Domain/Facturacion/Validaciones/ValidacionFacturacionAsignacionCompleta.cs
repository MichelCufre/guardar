using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Expedicion;
using WIS.Domain.Picking;

namespace WIS.Domain.Facturacion.Validaciones
{
    public class ValidacionFacturacionAsignacionCompleta : IFacturacionValidacion
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IValidacionFacturacionResultFormatResolver _resultResolver;
        protected string Message { get; set; }
        protected List<Pedido> PedidosError { get; set; }

        public ValidacionFacturacionAsignacionCompleta(IUnitOfWork uow, IValidacionFacturacionResultFormatResolver resolver)
        {
            this._uow = uow;
            this._resultResolver = resolver;
            this.PedidosError = new List<Pedido>();
            this.Message = "Existen contenedores no asignados";
        }

        public virtual void Validate(Camion camion, Pedido pedido)
        {
            if (_uow.PreparacionRepository.AnyContenedorNoAsignadoPedido(pedido.Empresa, pedido.Cliente, pedido.Id, camion.Id))
                this.PedidosError.Add(pedido);
        }

        public virtual bool IsValid()
        {
            return !this.PedidosError.Any();
        }

        public virtual ValidacionCamionResultado GetResult()
        {
            return this._resultResolver.Resolve(this.Message, this.PedidosError);
        }
    }
}