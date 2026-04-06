using System;

namespace WIS.Domain.Eventos
{
    public class NotificacionArchivo {
        public int Id { get; set; }

        public long NuEventoNotificacion { get; set; }

        public string DsArchivo { get; set; }

        public string IdReferencia { get; set; }

        public string TpReferencia { get; set; }

        public byte[] VlData { get; set; }

        public DateTime DtAddRow { get; set; }

        public DateTime? DtUpdateRow { get; set; }

        public NotificacionEmail Notificacion { get; set; }
    }
}
