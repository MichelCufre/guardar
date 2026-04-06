namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_HIST_PRDC_INGRESO_PASADA")]
    public partial class T_HIST_PRDC_INGRESO_PASADA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_HISTORICO { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string NU_PRDC_INGRESO { get; set; }

        public int QT_PASADAS { get; set; }

        public int CD_ACCION_INSTANCIA { get; set; }

        [Required]
        [StringLength(1000)]
        [Column]
        public string VL_ACCION_INSTANCIA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public int? NU_ORDEN { get; set; }

        public int? NU_FORMULA_ENSAMBLADA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_PRDC_LINEA { get; set; }

        public DateTime? DT_ADDHIST { get; set; }
    }
}
