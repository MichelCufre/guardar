namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    [Table("V_REC300_MOTIVO_ALMACENAMIENTO")]
    public partial class V_REC300_MOTIVO_ALMACENAMIENTO
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_MOTIVO_ALMACENAMIENTO { get; set; }

        public int NU_AGENDA { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_ETIQUETA_LOTE { get; set; }

        public int? NU_RECEPCION { get; set; }

        public int? NU_RECEPCION_DET { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public DateTime? DT_VENCIMIENTO { get; set; }

        [StringLength(3)]
        [Column]
        public string CD_MOTIVO { get; set; }

        public decimal? QT_REMITO { get; set; }

        public decimal? QT_ALMACENADA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        public DateTime? DT_ALMACENADA { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_EXTERNO_ETIQUETA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
