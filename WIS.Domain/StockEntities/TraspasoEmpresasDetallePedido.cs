using System;

namespace WIS.Domain.StockEntities
{
    public class TraspasoEmpresasDetallePedido
    {
        public long Id { get; set; } // NU_TRASPASO_DET_PEDIDO

        public long Traspaso { get; set; } // NU_TRASPASO

        public string PedidoOrigen { get; set; } // NU_PEDIDO

        public string ClienteOrigen { get; set; } // CD_CLIENTE

        public int EmpresaOrigen { get; set; } // CD_EMPRESA

        public string PedidoDestino { get; set; } // NU_PEDIDO_DESTINO

        public string ClienteDestino { get; set; } // CD_CLIENTE_DESTINO

        public int EmpresaDestino { get; set; } // CD_EMPRESA_DESTINO

        public string TipoPedidoDestino { get; set; } // TP_PEDIDO_DESTINO

        public string TipoExpedicionDestino { get; set; } // TP_EXPEDICION_DESTINO

        public DateTime? FechaAlta { get; set; } // DT_ADDROW
    }
}
