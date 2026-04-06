using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class PRE350StockPickingReabast
    {
        public int Empresa { get; set; }
        public string NombreEmpresa { get; set; }
        public string Producto { get; set; }
        public string DescProducto { get; set; }
        public decimal Faixa { get; set; }
        public string NuPredioPi { get; set; }
        public string EnderecoPicking { get; set; }
        public int? QtMinimoPi { get; set; }
        public int? QtMaximoPi{ get; set; }
        public decimal QtPadraoPi{ get; set; }
        public int? QtDesbordePi{ get; set; }
        public string IdentificadorPi { get; set; }
        public decimal? QtFisicoPi { get; set; }
        public decimal? QtSalidaPi { get; set; }
        public decimal? QtEntradaPi { get; set; }
        public int? QtDisponiblePi { get; set; }
        public bool FlNecesitaUrgente { get; set; }
        public bool FlNecesitaMinimo { get; set; }
        public bool FlNecesitaForzado { get; set; }
        public decimal? QtDisponibleStock { get; set; }
        public string VlPorcentajeForzado { get; set; }
    }
}
