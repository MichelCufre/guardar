using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_ORT_INSUMO_MANIPULEO_EMPRESA")]
    public partial class T_ORT_INSUMO_MANIPULEO_EMPRESA
    {
        [Required]
        [StringLength(10)]
        public string CD_INSUMO_MANIPULEO { get; set; }

        public int CD_EMPRESA { get; set; }

    }
}
