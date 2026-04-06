using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("I_E_PRDC_SALIDA_PRD")]
    public partial class I_E_PRDC_SALIDA_PRD
    {
        [Required]
        [StringLength(1)]
        public string ID_PROCESADO { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string NU_REGISTRO { get; set; }

        public DateTime? DT_ADDROW_INTERFAZ { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_PRDC_INGRESO { get; set; }
    }

}
