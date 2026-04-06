using System;
using WIS.Domain.Integracion;

namespace WIS.Domain.Automatismo
{
    public class AutomatismoInterfaz
    {
        public int Id { get; set; }
        public int? IdIntegracionServicio { get; set; }
        public int IdAutomatismo { get; set; }
        public int InterfazExterna { get; set; }
        public int? Interfaz { get; set; }
        public ServiceHttpProtocol ProtocoloComunicacion { get; set; }
        public string Method { get; set; }
        public string IdProtocoloComunicacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? Transaccion { get; set; }
        public IntegracionServicio IntegracionServicio { get; set; }
        public Automatismo Automatismo { get; set; }
    }
}
