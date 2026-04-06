namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_ACCION_INSTANCIA_KIT120")]
    public partial class V_ACCION_INSTANCIA_KIT120
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_ACCION_INSTANCIA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ACCION_INSTANCIA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(100)]
        [Column]
        public string VL_PARAMETRO1 { get; set; }

        [StringLength(100)]
        [Column]
        public string VL_PARAMETRO2 { get; set; }

        [StringLength(100)]
        [Column]
        public string VL_PARAMETRO3 { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string CD_ACCION { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ACCION { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string TP_ACCION { get; set; }
    }
}
