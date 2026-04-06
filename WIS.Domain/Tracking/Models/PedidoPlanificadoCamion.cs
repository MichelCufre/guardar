using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class PedidoPlanificadoCamion
    {
        public int Camion { get; set; }                 //CD_CAMION                     
        public string CodigoCliente { get; set; }       //CD_CLIENTE
        public int Empresa { get; set; }                //CD_EMPRESA                     
        public string Pedido { get; set; }              //NU_PEDIDO                     
        public string PuntoEntrega { get; set; }        //CD_PUNTO_ENTREGA                     
        public int? NuOrdenEntrega { get; set; }        //NU_ORDEN_ENTREGA                     
        public string Predio { get; set; }              //NU_PREDIO
        public string Sincronizado { get; set; }        //PEDIDO_SINCRONIZADO
        public string TipoExpedicion { get; set; }      //TP_EXPEDICION
        public string TpExpManejaTracking{ get; set; }  //TP_EXP_MANEJA_TRACKING
        public short? NuOrdenCarga { get; set; }        //NU_PRIOR_CARGA        
        public string CodigoAgente { get; set; }        //CD_AGENTE
        public string TipoAgente { get; set; }          //TP_AGENTE
    }
}
