using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Expedicion
{
    public class ContenedorAsociarUnidad
    {
        public string Cliente { get; set; }
        public int Empresa { get; set; }
        public int Preparacion { get; set; }
        public int Contenedor { get; set; }
        public long Carga { get; set; }
        public string GrupoExpedicion { get; set; }
        public short? Ruta { get; set; }
        public long? CargaDestino { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int Camion { get; set; }
        public string TipoModalidadArmado { get; set; }
        public long Transaccion { get; set; }
        public string FlSyncRealizada { get; set; }
        public string IdCarga { get; set; }
    }
}
