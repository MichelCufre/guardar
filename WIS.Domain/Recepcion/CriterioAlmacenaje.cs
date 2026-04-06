using System.Collections.Generic;
using WIS.Domain.General;

namespace WIS.Domain.Recepcion
{
    public class CriterioAlmacenaje
    {
        public string Referencia { get; set; }
        public string Predio { get; set; }
        public AgrupadorAlmacenaje Agrupador { get; set; }
        public string Grupo { get; set; }
        public List<ProductoAlmacenaje> Productos { get; set; }
        public string Clase { get; set; }
        public OperativaAlmacenaje Operativa { get; set; }
        public short? Pallet { get; set; }

        public CriterioAlmacenaje()
        {
            Agrupador = new AgrupadorAlmacenaje();
            Productos = new List<ProductoAlmacenaje>();
            Operativa = new OperativaAlmacenaje();
        }
    }
}
