using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_ORT_INSUMOS_WORT020")]
    public partial class V_ORT_INSUMOS_WORT020
    {

        [Required]
        [StringLength(10)]
        public string CD_INSUMO_MANIPULEO { get; set; }

        [StringLength(60)]
        public string DS_INSUMO_MANIPULEO { get; set; }

        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }

        [StringLength(1)]
        public string TP_INSUMO_MANIPULEO { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }
        public int? CD_EMPRESA { get; set; }

        [StringLength(1)]
        public string FL_TODA_EMPRESA { get; set; }

    }
}
