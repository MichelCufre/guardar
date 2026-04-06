using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Expedicion
{
    public class CargaAsociarUnidad
    {
        public string Cliente { get; set; }
        public int Preparacion { get; set; }
        public int Empresa { get; set; }
        public long Carga { get; set; }
        public string GrupoExpedicion { get; set; }
        public int Camion { get; set; }
        public DateTime FechaAlta { get; set; }
        public string TipoModalidadArmado { get; set; }
        public string IdCarga { get; set; }
        public string FlSyncRealizada { get; set; }
    }
}
