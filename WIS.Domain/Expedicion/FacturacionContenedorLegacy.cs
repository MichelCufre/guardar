using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Picking;

namespace WIS.Domain.Expedicion
{
    public class FacturacionContenedorLegacy
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Camion _camion;

        public FacturacionContenedorLegacy(IUnitOfWork uow, Camion camion)
        {
            this._uow = uow;
            this._camion = camion;
        }

        public virtual bool Facturar()
        {
            bool TieneFacturacion = false;

            var cargas = this._uow.CargaCamionRepository.GetsCargasCamion(this._camion);

            foreach (var carga in cargas)
            {
                List<Contenedor> contenedores = this._uow.ContenedorRepository.GetContenedoresCarga(carga);

                foreach (var contenedor in contenedores)
                {
                    DetallePreparacion detPick = this._uow.PreparacionRepository.GetDetallePreparacion(contenedor.NumeroPreparacion, contenedor.Numero);

                    Pedido pedido = this._uow.PedidoRepository.GetPedido(detPick.Empresa, detPick.Cliente, detPick.Pedido);

                    contenedor.CamionFacturado = 0;

                    if (pedido.ConfiguracionExpedicion.IsFacturacionRequerida)
                    {
                        TieneFacturacion = true;

                        //Registrar camion facturado en contenedor
                        contenedor.CamionFacturado = this._camion.Id;
                    }

                    contenedor.NumeroTransaccion = this._uow.GetTransactionNumber();

                    this._uow.ContenedorRepository.UpdateContenedor(contenedor);
                }
            }

            return TieneFacturacion;
        }
    }
}
