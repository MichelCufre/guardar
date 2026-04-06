using System;
using System.Linq;
using WIS.Domain.General;

namespace WIS.Domain.Automatismo
{
    public class AutomatismoPosicion
    {
        public int Id { get; set; }
        public string IdUbicacion { get; set; }
        public int IdAutomatismo { get; set; }
        public string TipoUbicacion { get; set; }
        public string PosicionExterna { get; set; }
        public short Orden { get; set; }
        public int? TipoAgrupacion { get; set; }
        public string ComparteAgrupacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? Transaccion { get; set; }
        public virtual Automatismo Automatismo { get; set; }
        public virtual Ubicacion Ubicacion { get; set; }

        public virtual int GetNumeroVirtualEtiqueta()
        {
            return int.Parse($"{this.IdAutomatismo}{this.Id}");
        }

        public virtual void LimpiarComparteAgrupacion()
        {
            this.ComparteAgrupacion = null;
        }

        public virtual string GetComparteEntrega()
        {
            if (this.ComparteAgrupacion == null) return null;

            return this.ComparteAgrupacion?.Split("#").FirstOrDefault();
        }
    }
}
