using System;

namespace WIS.Domain.General
{
    public class ZonaUbicacion
    {
        public string Id { get; set; }
        public string Descripcion{ get; set; }
        public string TipoZonaUbicacion { get; set; }
        public string ZonaUbicacionPicking { get; set; }
        public string Estacion { get; set; }
        public string EstacionAlmacenado { get; set; }
        public bool Habilitada { get; set; }
        public DateTime? Alta { get; set; }
        public DateTime? Modificacion { get; set; }
        public int IdInterno { get; set; }

        #region API

        public string HabilitadaId { get; set; }        
        public string Ubicacion { get; set; }        

        #endregion
    }
}
