using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class PlanificacionCamion
    {
        public int Camion { get; set; }                         //CD_CAMION
        public string PuntoEntrega { get; set; }                //CD_PUNTO_ENTREGA
        public string ComparteContenedorEntrega { get; set; }   //VL_COMPARTE_CONTENEDOR_ENTREGA
        public int Empresa { get; set; }                        //CD_EMPRESA
        public string Cliente { get; set; }                     //CD_CLIENTE
        public long Numero { get; set; }                        //NU_CONTENEDOR
        public string CodigoAgente { get; set; }                //CD_AGENTE
        public string TipoAgente { get; set; }                  //TP_AGENTE
        public string DescContenedor { get; set; }              //DS_CONTENEDOR
        public string TipoBulto { get; set; }                   //TP_BULTO        
        public decimal? CantidadBulto { get; set; }             //QT_PRODUTO
        public decimal? VolumenTotal { get; set; }              //VL_CUBAGEM_TOTAL
        public decimal? PesoTotal { get; set; }                 //VL_PESO_TOTAL
        public decimal? Alto { get; set; }                      //VL_ALTURA
        public decimal? Largo { get; set; }                     //VL_LARGURA
        public decimal? Produndidad { get; set; }               //VL_PROFUNDIDADE
        public short OrdenDeCarga { get; set; }                 //NU_PRIOR_CARGA
        public string TipoContenedor { get; set; }              //TP_CONTENEDOR
        public string IdExterno { get; set; }                   //ID_EXTERNO_CONTENEDOR
        public long? IdExternoTracking { get; set; }            //ID_EXTERNO_CONTENEDOR
        public string CodigoBarras { get; set; }                //CD_BARRAS
    }
}
