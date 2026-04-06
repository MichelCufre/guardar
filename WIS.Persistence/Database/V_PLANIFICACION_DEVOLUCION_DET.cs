
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{
    [Table("V_PLANIFICACION_DEVOLUCION_DET")]
    public partial class V_PLANIFICACION_DEVOLUCION_DET
    {
        [Key]
        [Column(Order = 0)]
        public int NU_AGENDA { get; set; }

        [Key]
        [Column(Order = 1)]
        public int CD_EMPRESA { get; set; }

        public string CD_EXTERNO { get; set; }

        [StringLength(50)]
        public string TP_LINEA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(4000)]
        public string CD_BARRAS { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 8)]
        public decimal CD_FAIXA { get; set; }

        public decimal? QT_AGENDADO { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        [StringLength(200)]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO4 { get; set; }

    }
}
