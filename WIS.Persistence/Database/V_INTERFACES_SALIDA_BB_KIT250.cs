using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_INTERFACES_SALIDA_BB_KIT250")]
    public partial class V_INTERFACES_SALIDA_BB_KIT250
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_PRDC_INGRESO { get; set; }

        [StringLength(100)]
        public string NM_PRDC_DEFINICION { get; set; }

        [Required]
        [StringLength(10)]
        public string CD_PRDC_LINEA { get; set; }

        [StringLength(40)]
        public string CD_CGC_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        public DateTime? DT_COMIENZO { get; set; }

        public DateTime? DT_SITUACAO { get; set; }

        public int? CD_EMPRESA { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(200)]
        public string DS_REFERENCIA { get; set; }

        [StringLength(1)]
        public string FL_ERROR_CARGA { get; set; }

        [StringLength(1)]
        public string FL_ERROR_PROCEDIMIENTO { get; set; }
    }
}
