using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_NOTIFICACIONES")]
    public partial class T_NOTIFICACIONES
    {
        [Key]
        public long ID_NOTIFICACION { get; set; }
        public string VL_CATEGORIA { get; set; }
        public string VL_NIVEL { get; set; }
        public string VL_ESTADO { get; set; }
        public string DS_MENSAJE { get; set; }
        public string VL_SERIALIZADO { get; set; }
    }
}
