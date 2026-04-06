using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_TRASPASO_CONF_DESTINO")]
    public partial class T_TRASPASO_CONF_DESTINO
    {
        public long NU_TRASPASO_CONF_DESTINO { get; set; }

        public long NU_TRASPASO_CONFIGURACION { get; set; }

        public int CD_EMPRESA { get; set; }

    }
}
