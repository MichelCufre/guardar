using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_ORT_ORDEN_TAREA_DATO")]
    public partial class T_ORT_ORDEN_TAREA_DATO
    {
        public decimal QT_INSUMO_MANIPULEO { get; set; }

        [StringLength(100)]
        public string DS_REFERENCIA { get; set; }

        [Required]
        [StringLength(10)]
        public string CD_INSUMO_MANIPULEO { get; set; }

        public long NU_ORT_ORDEN_TAREA_DATO { get; set; }

        public long NU_ORDEN_TAREA { get; set; }
    }
}