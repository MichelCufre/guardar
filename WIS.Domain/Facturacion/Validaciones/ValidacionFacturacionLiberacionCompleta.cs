using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Expedicion;
using WIS.Domain.Picking;

namespace WIS.Domain.Facturacion.Validaciones
{
    public class ValidacionFacturacionLiberacionCompleta : IFacturacionValidacion
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IValidacionFacturacionResultFormatResolver _resultResolver;
        public string Message { get; private set; }
        public List<Pedido> PedidosError { get; private set; }

        public ValidacionFacturacionLiberacionCompleta(IUnitOfWork uow, IValidacionFacturacionResultFormatResolver resolver)
        {
            this._uow = uow;
            this._resultResolver = resolver;
            this.PedidosError = new List<Pedido>();
            this.Message = "No fue liberado completamente";
        }

        public virtual void Validate(Camion camion, Pedido pedido)
        {
            if (!pedido.FueLiberadoCompletamente(out decimal cantidadLiberada))
                this.PedidosError.Add(pedido);

            /*List<DetallePreparacion> lineasPicking = this._uow.PreparacionRepository.GetDetallePreparacionByPedido(pedido.Empresa, pedido.Cliente, pedido.Id);

            if (lineasPicking.Count == 0)
                this.PedidosError.Add(pedido);

            decimal cantidadProducto = lineasPicking.Sum(d => d.Cantidad);

            if (cantidadLiberada != cantidadProducto)
                this.PedidosError.Add(pedido);*/
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
