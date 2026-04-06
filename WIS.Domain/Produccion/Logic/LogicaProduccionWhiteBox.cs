using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Produccion.Models;
using WIS.Security;

namespace WIS.Domain.Produccion.Logic
{
    public class LogicaProduccionWhiteBox : LogicaProduccion
    {
        public LogicaProduccionWhiteBox(IUnitOfWork uow, IIdentityService identity, IngresoProduccionWhiteBox ingresoProduccion)
            : base(uow, identity, ingresoProduccion)
        {
        }

        public override void ConsumirInsumoCompleto(long idInsumo, string ubicacion, decimal cantidadConsumir, out DateTime? vencimiento, bool isConsumible = false)
        {
            throw new NotImplementedException();
        }

        public override void ConsumirInsumoParcial(long idInsumo, string ubicacion, decimal qtConsumir, out DateTime? vencimiento, bool isConsumible = false)
        {
            throw new NotImplementedException();
        }

        public override IngresoProduccion CrearIngresoProduccion(string tipoIngreso, int empresa, string predio, List<IngresoProduccionDetalleTeorico> detalles, string idExterno = null, string idEspacioProduccion = null)
        {
            throw new NotImplementedException();
        }

        public override void DefinirLotesPedido(Pedido pedido, List<DetallePedido> detalleDefinido, IFormatProvider format)
        {
            throw new NotImplementedException();
        }

        public override IngresoProduccionDetalleReal ExisteIngresoReal(string codigoProducto, string identificador)
        {
            throw new NotImplementedException();
        }

        public override void GenerarProductoNoEsperado(Producto producto, decimal faixa, int empresa, string lote, decimal producido, DateTime? vencimiento, string codMotivo, string dsMotivo, out string keyAjuste)
        {
            throw new NotImplementedException();
        }

        public override bool PuedeIniciarProduccion(out string mensaje, out List<string> errorArg)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePedido(Pedido pedido)
        {
            throw new NotImplementedException();
        }
    }
}
