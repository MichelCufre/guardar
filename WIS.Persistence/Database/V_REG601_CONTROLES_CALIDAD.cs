namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REG601_CONTROLES_CALIDAD")]
    public partial class V_REG601_CONTROLES_CALIDAD
    {
        [Key]
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

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
