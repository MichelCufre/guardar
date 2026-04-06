using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.General
{
    public class GrupoReglaParametro
    {
        public long Id { get; set; }                        //NU_GRUPO_REGLA_PARAM
        public long NroRegla { get; set; }                  //NU_GRUPO_REGLA
        public int NroParametro { get; set; }               //NU_PARAM
        public string Valor { get; set; }                   //VL_PARAM
        public DateTime? FechaInsercion { get; set; }       //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW
    }
}
