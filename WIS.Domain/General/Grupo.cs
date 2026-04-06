using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.General
{
    public class Grupo
    {
        public string Id { get; set; }                      //CD_GRUPO
        public string Descripcion { get; set; }             //DS_GRUPO
        public string CodigoClase { get; set; }             //CD_CLASSE
        public DateTime? FechaInsercion { get; set; }       //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW
        public bool Default { get; set; }                   //FL_DEFAULT
    }
}
