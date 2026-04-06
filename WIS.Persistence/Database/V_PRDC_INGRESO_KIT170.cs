namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRDC_INGRESO_KIT170")]
    public partial class V_PRDC_INGRESO_KIT170
    {
        [Key]
        [StringLength(10)]
        [Column]
        public string NU_PRDC_INGRESO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_GENERAR_PEDIDO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public int? QT_FORMULA { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [Required]
        [StringLength(50)]
        [Column]
        public string LOGINNAME { get; set; }

        public short? CD_SITUACAO { get; set; }

        [Required]
        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string CD_PRDC_DEFINICION { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_PRDC_DEFINICION { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_TIPO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_DOMINIO_VALOR { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO_VALOR { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_PRDC_LINEA { get; set; }

        [StringLength(2000)]
        [Column]
        public string DS_PRDC_LINEA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_ENTRADA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_SALIDA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_BLACKBOX { get; set; }
    }
}
