using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.General
{
    public class Pallet
    {
        public short Id { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaInsercion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
