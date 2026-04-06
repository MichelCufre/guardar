namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_PRODUCTO_PALLET_WREG605")]
    public partial class V_PRODUCTO_PALLET_WREG605
    {
        [Key]
        [Required]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Required]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Required]
        public short CD_PALLET { get; set; }

        [Key]
        [Required]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(60)]
        public string DS_PALLET { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Required]
        public decimal QT_UNIDADES { get; set; }

        [Required]
        public short NU_PRIORIDAD { get; set; }

    }
}
