using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Recepcion
{
    public class RecepcionAgendaReferencia
    {
        public int Empresa { get; set; }                        //CD_EMPRESA
        public decimal Faixa { get; set; }                      //CD_FAIXA
        public string CodigoProducto { get; set; }              //CD_PRODUTO
        public DateTime? FechaAlta { get; set; }                //DT_ADDROW
        public int Agenda { get; set; }                         //NU_AGENDA
        public string Identificador { get; set; }               //NU_IDENTIFICADOR
        public long? NumeroInterfazEjecucion { get; set; }      //NU_INTERFAZ_EJECUCION
        public int IdDetalleReferencia { get; set; }            //NU_RECEPCION_REFERENCIA_DET
        public decimal? CantidadAgendada { get; set; }           //QT_AGENDADO
        public decimal? CantidadRecibida { get; set; }           //QT_RECIBIDA
    }
}
