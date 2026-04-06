using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class PlanificacionDevolucion
    {
        public int Agenda { get; set; }                         //NU_AGENDA
        public int Empresa { get; set; }                        //CD_EMPRESA
        public string CodigoAgente { get; set; }                //CD_AGENTE
        public string TipoAgente { get; set; }                  //TP_AGENTE
        public int Numero { get; set; }                         //NU_CONTENEDOR
        public string DescripcionContenedor { get; set; }       //DS_CONTENEDOR
        public string TipoBulto { get; set; }                   //TP_BULTO        
        public decimal? CantidadBulto { get; set; }             //QT_AGENDADO
        public decimal? VolumenTotal { get; set; }              //VL_CUBAGEM_TOTAL
        public decimal? PesoTotal { get; set; }                 //VL_PESO_TOTAL
        public decimal? Alto { get; set; }                      //VL_ALTURA
        public decimal? Largo { get; set; }                     //VL_LARGURA
        public decimal? Produndidad { get; set; }               //VL_PROFUNDIDADE
        public string TipoReferencia { get; set; }              //TP_REFERENCIA
        public string CodigoReferencia { get; set; }            //CD_REFERENCIA
        public string TipoContenedor { get; set; }              //TP_CONTENEDOR
        public string Telefono { get; set; }                    //NU_TELEFONO
        public DateTime? FechaPrometida{ get; set; }            //DT_PROMETIDA
    }
}
