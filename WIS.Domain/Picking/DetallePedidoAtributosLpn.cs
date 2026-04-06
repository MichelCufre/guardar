using System;
using System.Collections.Generic;

namespace WIS.Domain.Picking
{
    public class DetallePedidoAtributosLpn
    {
        public long IdConfiguracion { get; set; }               // NU_DET_PED_SAI_ATRIB
        public string Pedido { get; set; }                      // NU_PEDIDO
        public string Cliente { get; set; }                     // CD_CLIENTE
        public int Empresa { get; set; }                        // CD_EMPRESA
        public string Producto { get; set; }                    // CD_PRODUTO
        public int Faixa { get; set; }                          // CD_FAIXA
        public string Identificador { get; set; }               // NU_IDENTIFICADOR
        public string IdEspecificaIdentificador { get; set; }   // ID_ESPECIFICA_IDENTIFICADOR
        public decimal CantidadPedida { get; set; }             // QT_PEDIDO
        public decimal? CantidadLiberada { get; set; }          // QT_LIBERADO
        public decimal? CantidadAnulada { get; set; }           // QT_ANULADO
        public string IdLpnExterno { get; set; }                // ID_LPN_EXTERNO
        public string Tipo { get; set; }                        // TP_LPN_TIPO
        public long? Transaccion { get; set; }                  // NU_TRANSACCION
        public DateTime FechaAlta { get; set; }                 // DT_ADDROW
        public DateTime? FechaModificacion { get; set; }        // DT_UPDROW
        public Guid InternalId { get; set; }

        public List<DetallePedidoConfigAtributo> Atributos { get; set; }

        public DetallePedidoAtributosLpn()
        {
            Atributos = new List<DetallePedidoConfigAtributo>();
        }
    }
}
