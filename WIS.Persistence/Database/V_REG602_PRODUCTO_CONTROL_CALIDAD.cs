namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REG602_PRODUCTO_CONTROL_CALIDAD")]
    public partial class V_REG602_PRODUCTO_CONTROL_CALIDAD
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_CONTROL { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string DS_CONTROL { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string SG_CONTROL { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string ID_BLOQUEIO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
