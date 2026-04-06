using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General.Enums;

namespace WIS.Domain.General
{
    public class Herramienta
    {
        public short Id { get; set; }                   //CD_FERRAMENTA
        public short Estado { get; set; }               //CD_SITUACAO
        public string Descripcion { get; set; }         //DS_FERRAMENTA
        public bool Autoasignado { get; set; }          //ID_AUTOASIGNADO
        public DateTime FechaInsercion { get; set; }    //DT_UPDROW
        public DateTime FechaModificacion { get; set; } //DT_ADDROW
    }
}
