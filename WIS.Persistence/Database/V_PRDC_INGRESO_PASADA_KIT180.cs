namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRDC_INGRESO_PASADA_KIT180")]
    public partial class V_PRDC_INGRESO_PASADA_KIT180
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_PRDC_INGRESO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int QT_PASADAS { get; set; }

        public int? CD_ACCION_INSTANCIA { get; set; }

        [Required]
        [StringLength(1000)]
        [Column]
        public string VL_ACCION_INSTANCIA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_ORDEN { get; set; }

        public int? NU_FORMULA_ENSAMBLADA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_PRDC_LINEA { get; set; }
    }
}
