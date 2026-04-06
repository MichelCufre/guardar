namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRDC_INGRESO_KIT150")]
    public partial class V_PRDC_INGRESO_KIT150
    {
        [Key]
        [StringLength(10)]
        [Column]
        public string NU_PRDC_INGRESO { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string CD_PRDC_DEFINICION { get; set; }

        public int? QT_FORMULA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_GENERAR_PEDIDO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public short? CD_SITUACAO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_INTERFAZ_EJECUCION_ENTRADA { get; set; }

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

        [StringLength(10)]
        [Column]
        public string NU_PRDC_ORIGINAL { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_TIPO { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_PRDC_DEFINICION { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO_EGR { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_DOCUMENTO_EGR { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO_ING { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_DOCUMENTO_ING { get; set; }

        [Required]
        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_FUNCIONARIO { get; set; }

        public decimal? QT_PEDIDO { get; set; }

        public decimal? QT_LIBERADO { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        public decimal? QT_LINEA { get; set; }

        public decimal? QT_ELABORADO { get; set; }
    }
}
