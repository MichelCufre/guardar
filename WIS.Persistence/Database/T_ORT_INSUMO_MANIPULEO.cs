using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("T_ORT_INSUMO_MANIPULEO")]
    public partial class T_ORT_INSUMO_MANIPULEO
    {

        [Required]
        [StringLength(10)]
        public string CD_INSUMO_MANIPULEO { get; set; }

        [StringLength(60)]
        public string DS_INSUMO_MANIPULEO { get; set; }

        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }

        [Required]
        [StringLength(1)]
        public string TP_INSUMO_MANIPULEO { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_TODA_EMPRESA { get; set; }

    }
}
