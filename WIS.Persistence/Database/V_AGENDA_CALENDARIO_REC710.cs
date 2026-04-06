namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_AGENDA_CALENDARIO_REC710")]
    public partial class V_AGENDA_CALENDARIO_REC710
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_CIERRE { get; set; }

        public DateTime? DT_ENTREGA { get; set; }

        [StringLength(40)]
        [Column]
        public string HR_INICIO { get; set; }

        [StringLength(40)]
        [Column]
        public string HR_FIN { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        public int? CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        public short? CD_PORTA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_PORTA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO4 { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_PLACA { get; set; }

        public long? NU_INTERFAZ_EJECUCION { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_RECEPCION { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_RECEPCION_EXTERNO { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(3)]
        [Column]
        public string CD_TIPO_DOCUMENTO { get; set; }

        public decimal? QT_UNIDADES_TOTAL { get; set; }

        public decimal? QT_CAJAS_TOTAL { get; set; }

        public decimal? QT_PESO_TOTAL { get; set; }

        public decimal? QT_VOLUMEN_TOTAL { get; set; }
    }
}
