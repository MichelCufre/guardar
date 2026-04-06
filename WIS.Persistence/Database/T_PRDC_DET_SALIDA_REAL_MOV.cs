using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("T_PRDC_DET_SALIDA_REAL_MOV")]
    public partial class T_PRDC_DET_SALIDA_REAL_MOV
    {

        [Key]
        [Column(Order = 0)]
        public long NU_SALIDA_REAL_MOV { get; set; }

        [Required]
        [StringLength(50)]
        public string NU_PRDC_INGRESO { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(100)]
        public string NU_IDENTIFICADOR { get; set; }

        public int CD_EMPRESA { get; set; }

        public decimal CD_FAIXA { get; set; }

        public decimal QT_ESTOQUE { get; set; }

        [StringLength(100)]
        public string ND_MOTIVO { get; set; }
        public long NU_TRANSACCION { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_VENCIMIENTO { get; set; }

        [StringLength(1)]
        public string FL_PENDIENTE_NOTIFICAR { get; set; }

        [StringLength(400)]
        public string DS_MOTIVO { get; set; }
    }
}