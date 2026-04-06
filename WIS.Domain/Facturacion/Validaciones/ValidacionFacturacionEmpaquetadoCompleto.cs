using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Expedicion;
using WIS.Domain.Picking;

namespace WIS.Domain.Facturacion.Validaciones
{
    public class ValidacionFacturacionEmpaquetadoCompleto : IFacturacionValidacion
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IValidacionFacturacionResultFormatResolver _resultResolver;
        public string Message { get; private set; }
        public List<Pedido> PedidosError { get; private set; }

        public ValidacionFacturacionEmpaquetadoCompleto(IUnitOfWork uow, IValidacionFacturacionResultFormatResolver resolver)
        {
            this._uow = uow;
            this._resultResolver = resolver;
            this.PedidosError = new List<Pedido>();
            this.Message = "Existen contenedores sin empaquetar";
        }

        public virtual void Validate(Camion camion, Pedido pedido)
        {
            var cargasCamion = _uow.CargaCamionRepository.GetsCargasCamion(camion);
            if (this._uow.PreparacionRepository.AnyContenedorNoEmpaquetado(cargasCamion, pedido.Empresa, pedido.Cliente, pedido.Id))
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
