using System;

namespace WIS.Domain.Eventos.Factories
{
    public class DestinatarioFactory
    {
        public virtual Destinatario Create(TipoDestinatario value)
        {
            switch (value)
            {

                case TipoDestinatario.Contacto:
                    return new Contacto();
                case TipoDestinatario.Funcionario:
                    return new UsuarioDestinatario();

            }

            throw new InvalidOperationException("General_Sec0_Error_Error01");
        }
    }
}
