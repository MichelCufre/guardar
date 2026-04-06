using System;

namespace WIS.Domain.General
{
    public class UbicacionExterna : Ubicacion
    {
        public string CodigoEmpresa { get; set; }
        public string CodigoTipoUbicacion { get; set; }
        public string CodigoRotatividad { get; set; }
        public string CodigoFamilia { get; set; }
        public string CodigoArea { get; set; }
        public string NuAltura { get; set; }
        public string NuColumna { get; set; }
        public string NuProfundidad { get; set; }
        public string CodigoControlAcceso { get; set; }

        public string IdUbicacionBaja { get; set; }
        public string IdNecesitaReabastecer { get; set; }
        public string IdEsUbicacionSeparacion { get; set; }
        public string ValorDefecto { get; set; }
        public bool IsRecorrible { get; set; }
    }
}
