using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.StockEntities
{
    public class FiltrosStock
    {
        public int Empresa { get; set; }
        public int Pagina { get; set; }
        public string Ubicacion { get; set; }
        public string Producto { get; set; }
        public string Clase { get; set; }
        public int? Familia { get; set; }
        public short? Ramo { get; set; }
        public string TipoManejoFecha { get; set; }
        public string ManejoIdentificador { get; set; }
        public string Predio { get; set; }
        public bool Averia { get; set; }
        public string GrupoConsulta { get; set; }
    }
}
