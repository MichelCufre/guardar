using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WIS.Persistence.Database
{
    [Table("V_INV416_REG_DISP")]

    public partial class V_INV416_REG_DISP
    {
        [Key]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [Key]
        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        public decimal? QT_ESTOQUE { get; set; }

        [StringLength(1)]
        public string ID_AVERIA { get; set; }

        [StringLength(1)]
        public string ID_INVENTARIO { get; set; }

        [StringLength(1)]
        public string ID_CTRL_CALIDAD { get; set; }

        [StringLength(20)]
        public string CD_MOTIVO_AVERIA { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        public decimal NU_INVENTARIO { get; set; }
        
        [Key]
        [StringLength(15)]
        public string NU_LPN { get; set; }
        
        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }
        
        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }
    }
}
