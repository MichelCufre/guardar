namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_CODIGO_BARRAS_WREG603")]
    public partial class V_CODIGO_BARRAS_WREG603
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string CD_BARRAS { get; set; }

        public int? TP_CODIGO_BARRAS { get; set; }

        [StringLength(40)]
        [Column]
        public string DS_CODIGO_BARRAS { get; set; }

        public short? NU_PRIORIDADE_USO { get; set; }
    }
}
