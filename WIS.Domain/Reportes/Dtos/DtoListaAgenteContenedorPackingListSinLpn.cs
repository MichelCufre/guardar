using System.Collections.Generic;
using System.Linq;

namespace WIS.Domain.Reportes.Dtos
{
    public class DtoListaAgenteContenedorPackingListSinLpn
    {
        public DtoListaAgenteContenedorPackingListSinLpn()
        {
            Detalles = new List<DtoDetalleContenedorPackingListSinLpn>();
        }

        public string TipoContenedor { get; set; }
        public int Contenedor { get; set; }
        public int Bulto { get; set; }
        public string IdExternoContenedor { get; set; }


        public List<DtoDetalleContenedorPackingListSinLpn> Detalles { get; set; }

        public virtual decimal GetTotal()
        {
            return Detalles.Sum(d => d.Cantidad);
        }
    }
}
