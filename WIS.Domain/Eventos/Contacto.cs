using System;
using System.Collections.Generic;

namespace WIS.Domain.Eventos
{
    public partial class Contacto : Destinatario
    {
        public int CodigoEmpresa { get; set; }

        public string CodigoCliente { get; set; }

        public string Descripcion { get; set; }

        public string Email { get; set; }

        public string Telefono { get; set; }

        public int? IdUsuario { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public long? NumeroTransaccion { get; set; }

        public long? NumeroTransaccionDelete { get; set; }

        public List<GrupoContacto> Grupos { get; set; }

        public Contacto()
        {
            Grupos = new List<GrupoContacto>();
            this.TipoDestinatario = TipoDestinatario.Contacto;
        }

    }
}
