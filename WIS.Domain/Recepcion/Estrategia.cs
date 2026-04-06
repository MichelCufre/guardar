using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Recepcion
{
    public class Estrategia
    {
        public int NumeroEstrategia { get; set; }          //NU_ALM_ESTRATEGIA
        public short? Asociaciones { get; set; }     //QT_ASOCIACIONES
        public string Descripcion { get; set; }       //DS_ALM_ESTRATEGIA
        public DateTime FechaAdicion { get; set; } //DT_ADDROW
        public DateTime? FechaModificacion { get; set; } //DT_UPDROW
    }
}
