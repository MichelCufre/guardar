namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_STO060_CTR_CALIDAD")]
    public partial class V_STO060_CTR_CALIDAD
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_CTR_CALIDAD_PENDIENTE { get; set; }

        public int? CD_CONTROL { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string DS_CONTROL { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        public int? NU_ETIQUETA { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ACEPTADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? CD_FAIXA { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public int? CD_FUNCIONARIO_ACEPTO { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_UBICACION { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_EXTERNO_ETIQUETA { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_ETIQUETA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_NAM { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_MERCADOLOGICO { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_UNIDADE_MEDIDA { get; set; }

        public int? CD_FAMILIA_PRODUTO { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_CLASSE { get; set; }

        [StringLength(20)]
        [Column]
        public string DS_REDUZIDA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
