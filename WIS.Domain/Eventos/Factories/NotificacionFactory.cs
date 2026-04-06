using System;

namespace WIS.Domain.Eventos.Factories
{
    public class NotificacionFactory
    {
        public virtual Notificacion Create(TipoNotificacion value)
        {
            switch (value)
            {
                case TipoNotificacion.EMAIL:
                    return new NotificacionEmail();

            }

            throw new InvalidOperationException("General_Sec0_Error_Error01");
        }
    }
}
