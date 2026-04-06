using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Recepcion
{
    public class EtiquetaLoteDetalle
    {
        public int IdEtiquetaLote { get; set; }

        public string CodigoProducto { get; set; }

        public decimal Faixa { get; set; }

        public int IdEmpresa { get; set; }

        public string Identificador { get; set; }

        public decimal? CantidadRecibida { get; set; }

        public decimal? Cantidad { get; set; }

        public decimal? CantidadAjusteRecibido { get; set; }

        public decimal? CantidadEtiquetaGenerada { get; set; }

        public decimal? CantidadAlmacenada { get; set; }

        public DateTime? Vencimiento { get; set; }

        public DateTime? Insercion { get; set; }

        public DateTime? Modificacion { get; set; }

        public decimal? CantidadRastreoPallet { get; set; }

        public decimal? CantidadMovilizado { get; set; }

        public DateTime? Entrada { get; set; }

        public decimal? PesoRecibido { get; set; }

        public decimal? Peso { get; set; }

        public string DescripcionMotivo { get; set; }

        public long? NumeroTransaccion { get; set; }

        public EtiquetaLote EtiquetaLote { get; set; }

        public virtual string GetCompositeId()
        {
            return $"{IdEtiquetaLote}#{CodigoProducto}#{Identificador}#{Faixa.ToString("#.###")}#{IdEmpresa}";
        }
    }
}
