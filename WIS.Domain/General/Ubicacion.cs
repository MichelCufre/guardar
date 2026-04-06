using System;
using System.Linq;
using System.Text;
using WIS.Extension;

namespace WIS.Domain.General
{
    public class Ubicacion
    {
        public string Id { get; set; }
        public int IdEmpresa { get; set; }
        public short IdUbicacionTipo { get; set; }
        public short IdProductoRotatividad { get; set; }
        public int IdProductoFamilia { get; set; }
        public string CodigoClase { get; set; }
        public short IdUbicacionArea { get; set; }
        public string IdUbicacionZona { get; set; }
        public string IdControlAcceso { get; set; }
        public short CodigoSituacion { get; set; }
        public string CodigoControl { get; set; }
        public string FacturacionComponente { get; set; }
        public string NumeroPredio { get; set; }
        public string Bloque { get; set; }
        public string Calle { get; set; }
        public int? Columna { get; set; }
        public int? Altura { get; set; }
        public string DominioSector { get; set; }
        public bool EsUbicacionBaja { get; set; }
        public bool NecesitaReabastecer { get; set; }
        public DateTime FechaModificacion { get; set; }
        public DateTime FechaInsercion { get; set; }
        public bool EsUbicacionSeparacion { get; set; }
        public int? Profundidad { get; set; }
        public string CodigoBarras { get; set; }
        public long? OrdenDefecto { get; set; }

        public Empresa Empresa { get; set; }
        public UbicacionTipo UbicacionTipo { get; set; }
        public ProductoRotatividad ProductoRotatividad { get; set; }
        public ProductoFamilia ProductoFamilia { get; set; }
        public Clase Clase { get; set; }
        public UbicacionArea UbicacionArea { get; set; }
        public Predio Predio { get; set; }

        #region API
        public string IdUbicacionBaja { get; set; }
        public string IdNecesitaReabastecer { get; set; }
        public string IdUbicacionSeparacion { get; set; }
        #endregion

    }
}
