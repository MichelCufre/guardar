namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REC190_ETIQUETA_LOTE")]
    public partial class V_REC190_ETIQUETA_LOTE
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_ETIQUETA_LOTE { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string NU_EXTERNO_ETIQUETA { get; set; }

        [Required]
        [StringLength(3)]
        [Column]
        public string TP_ETIQUETA { get; set; }

        public int NU_AGENDA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        public short? CD_SITUACAO { get; set; }

        [Required]
        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        public int? CD_FUNC_ALMACENAMIENTO { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_FUNCIONARIO { get; set; }

        public DateTime? DT_RECEPCION { get; set; }

        public DateTime? DT_ALMACENAMIENTO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_AGENTE { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        public short? CD_PALLET { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_PALLET { get; set; }
    }
}
