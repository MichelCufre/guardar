using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class ContenedorEntrega
    {
        public int Contenedor { get; set; }             //NU_CONTENEDOR
        public string Cliente { get; set; }             //CD_CLIENTE
        public int Empresa { get; set; }                //CD_EMPRESA
        public string Pedido { get; set; }              //NU_PEDIDO
        public string TipoContenedor { get; set; }      //TP_CONTENEDOR
        public short? Situacion { get; set; }           //CD_SITUACAO
        public int Camion { get; set; }                 //CD_CAMION
        public string TpObjetoTracking { get; set; }    //TP_OBJETO_TRACKING
        public string IdExterno { get; set; }           //ID_EXTERNO_CONTENEDOR
        public decimal? CantidadBulto { get; set; }     //QT_PRODUTO
        public decimal? Volumen { get; set; }           //VL_CUBAGEM
        public decimal? PesoTotal { get; set; }         //VL_PESO_TOTAL
        public decimal? Alto { get; set; }              //VL_ALTURA
        public decimal? Largo { get; set; }             //VL_LARGURA
        public decimal? Produndidad { get; set; }       //VL_PROFUNDIDADE
        public long? IdExternoTracking { get; set; }    //ID_EXTERNO_TRACKING
        public string Descripcion { get; set; }         //DS_CONTENEDOR
        public string CodigoBarras { get; set; }        //CD_BARRAS
    }
}
