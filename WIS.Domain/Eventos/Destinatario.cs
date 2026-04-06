using System.Collections.Generic;

namespace WIS.Domain.Eventos
{
    public abstract partial class Destinatario
    {
        public Destinatario()
        {
            this.Instancias = new List<Instancia>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }

        public TipoDestinatario TipoDestinatario { get; set; }
        public List<Instancia> Instancias { get; set; }

        public static TipoDestinatario GetTipoDistinatario(string value)
        {
            switch (value) //TPEVDEST 
            {
                case "GPO": return TipoDestinatario.Grupo;
                case "CON": return TipoDestinatario.Contacto;
                case "FUN": return TipoDestinatario.Funcionario;
                default: return TipoDestinatario.Unknown;
            }
        }

        public static string GetTipoDistinatario(TipoDestinatario value)
        {
            switch (value)
            {
                case TipoDestinatario.Grupo: return "GPO";
                case TipoDestinatario.Contacto: return "CON";
                case TipoDestinatario.Funcionario: return "FUN";
                default: return "Unknown";
            }
        }

    }
}
