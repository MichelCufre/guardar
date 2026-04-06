using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_DOCUMENTO_SALDO_DETALLE")]
    public partial class V_DOCUMENTO_SALDO_DETALLE
    {
        [StringLength(6)]
        public string TP_DUA { get; set; }

        [StringLength(25)]
        public string NU_DUA { get; set; }

        [StringLength(20)]
        public string NU_AGRUPADOR { get; set; }

        [StringLength(3)]
        public string TP_AGRUPADOR { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_DOCUMENTO_INGRESO_DUA { get; set; }

        [Required]
        [StringLength(6)]
        public string TP_DOCUMENTO_INGRESO_DUA { get; set; }

        [StringLength(6)]
        public string ID_ESTADO { get; set; }

        public decimal? VL_ARBITRAJE { get; set; }

        public int? NU_AGENDA { get; set; }

        [StringLength(1)]
        public string ID_MANUAL { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 1)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        public decimal? QT_INGRESADA { get; set; }

        public decimal? QT_RESERVADA { get; set; }

        public decimal? QT_DESAFECTADA { get; set; }

        [StringLength(1)]
        public string ID_DISPONIBLE { get; set; }

        public DateTime? DT_DISPONIBLE { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public decimal? QT_MERCADERIA { get; set; }

        public decimal? QT_DISPONIBLE { get; set; }
    }

}
