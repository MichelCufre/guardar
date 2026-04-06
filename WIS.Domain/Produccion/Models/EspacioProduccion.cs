using System;
using WIS.Domain.General;

namespace WIS.Domain.Produccion.Models
{
    public abstract class EspacioProduccion
    {
        public string Id { get; set; }

        public string Descripcion { get; set; }

        public string Tipo { get; set; }

        public string IdUbicacionEntrada { get; set; }

        public string IdUbicacionSalida { get; set; }

        public string IdUbicacionSalidaTran { get; set; }

        public string IdUbicacionProduccion { get; set; }

        public Ubicacion UbicacionEntrada { get; set; }

        public Ubicacion UbicacionSalida { get; set; }

        public Ubicacion UbicacionSalidaTran { get; set; }

        public Ubicacion UbicacionProduccion { get; set; }

        public string Predio { get; set; }

        public DateTime? FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public string NumeroIngreso { get; set; }

        public string FlConfirmacionManual { get; set; }

        public string FlStockConsumible { get; set; }

        public long? NumeroTransaccion { get; set; }
    }
}