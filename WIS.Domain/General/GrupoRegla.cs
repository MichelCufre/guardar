using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.General
{
    public class GrupoRegla
    {
        public long Id { get; set; }                        //NU_GRUPO_REGLA
        public string Descripcion { get; set; }             //DS_REGLA
        public string CodigoGrupo { get; set; }             //CD_GRUPO
        public int Orden { get; set; }                      //NU_ORDEN
        public DateTime? FechaInsercion { get; set; }       //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW
    }
}
