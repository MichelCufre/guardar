using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.General
{
    public class GrupoParametro
    {
        public int Id { get; set; }                         //NU_PARAM
        public string Nombre { get; set; }                  //NM_PARAM
        public string Descripcion { get; set; }             //DS_PARAM
        public string ValorDefault { get; set; }            //VL_PARAM_DEFAULT
        public int? Orden { get; set; }                     //NU_ORDEN
        public string Tipo { get; set; }                    //TP_PARAM
        public DateTime? FechaInsercion { get; set; }       //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW
    }
}
