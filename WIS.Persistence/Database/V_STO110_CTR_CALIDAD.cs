namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_STO110_CTR_CALIDAD")]
    public partial class V_STO110_CTR_CALIDAD
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

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

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

        [Required]
        [StringLength(30)]
        [Column]
        public string NM_FUNCIONARIO { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_EXTERNO_ETIQUETA { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_ETIQUETA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public long? NU_LPN { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_LPN_EXTERNO { get; set; }

        [StringLength(40)]
        [Column]
        public string TP_LPN_TIPO { get; set; }

        public int? ID_LPN_DET { get; set; }

        [StringLength(40)]
        [Column]
        public string ID_LINEA_SISTEMA_EXTERNO { get; set; }

        public int? NU_AGENDA { get; set; }

    }
}
