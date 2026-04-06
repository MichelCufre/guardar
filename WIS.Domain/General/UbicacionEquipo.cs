using System;

namespace WIS.Domain.General
{
    public class UbicacionEquipo
    {
        public int CodigoEquipo { get; set; }
        public string Ubicacion { get; set; }
        public string Predio { get; set; }
        public int Usuario { get; set; }
        public string AutoAsignado { get; set; }
    }
}
