using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class PlanificacionDevolucionDetalle
    {
        public int Agenda{ get; set; }                      //NU_AGENDA 
        public int Empresa { get; set; }                    //CD_EMPRESA
        public string CodigoExterno { get; set; }           //CD_EXTERNO
        public string TipoLinea { get; set; }               //TP_LINEA
        public string CodigoProducto { get; set; }          //CD_PRODUTO
        public string CodigoBarras { get; set; }            //CD_BARRAS
        public string DescipcionProducto { get; set; }      //DS_PRODUTO
        public string Identificador { get; set; }           //NU_IDENTIFICADOR
        public decimal Faixa { get; set; }                  //CD_FAIXA
        public decimal? CantidadAgendada { get; set; }      //QT_AGENDADO
        public DateTime? FechaVencimiento { get; set; }     //DT_FABRICACAO
        public string Anexo1 { get; set; }                  //DS_ANEXO1
        public string Anexo2 { get; set; }                  //DS_ANEXO2
        public string Anexo3 { get; set; }                  //DS_ANEXO3
        public string Anexo4 { get; set; }                  //DS_ANEXO4
    }
}
