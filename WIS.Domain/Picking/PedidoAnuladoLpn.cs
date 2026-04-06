using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Expedicion;

namespace WIS.Domain.Picking
{
    public class PedidoAnuladoLpn
    {
        public long Id { get; set; }                        //NU_LOG_PEDIDO_ANULADO_LPN
        public long IdLogPedidoAnulado { get; set; }        //NU_LOG_PEDIDO_ANULADO
        public string TipoOperacion { get; set; }           //TP_OPERACION
        public string IdExternoLpn { get; set; }            //ID_LPN_EXTERNO
        public string TipoLpn { get; set; }                 //TP_LPN_TIPO
        public long? IdConfiguracion { get; set; }          //NU_DET_PED_SAI_ATRIB
        public decimal CantidadAnulada { get; set; }        //QT_ANULADO
        public DateTime FechaInsercion { get; set; }        //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }  //DT_ADDROW

    }
}
