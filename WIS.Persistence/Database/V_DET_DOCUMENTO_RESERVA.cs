using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_DET_DOCUMENTO_RESERVA")]
    public partial class V_DET_DOCUMENTO_RESERVA
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [StringLength(22)]
        public string NU_DOCUMENTO_FORMAT { get; set; }

        [Required]
        [StringLength(6)]
        public string TP_DOCUMENTO_INGRESO { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_DOCUMENTO_INGRESO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(6)]
        public string ID_ESTADO { get; set; }

        public DateTime? DT_FINALIZADO { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime DT_ADDROW { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(20)]
        public string CD_NAM { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(15)]
        public string TP_DUA_INGRESO { get; set; }

        [StringLength(25)]
        public string NU_DUA_INGRESO { get; set; }

        [StringLength(20)]
        public string NU_AGRUPADOR { get; set; }

        [StringLength(3)]
        public string TP_AGRUPADOR { get; set; }

        public decimal? VL_MERCADERIA { get; set; }

        public decimal? QT_RESERVADA { get; set; }

        public decimal? QT_DOCUMENTO { get; set; }

        public decimal? VL_CIF_INGRESO { get; set; }

        public int? VL_FOB_INGRESO { get; set; }

        public decimal? VL_TRIBUTO { get; set; }
    }
}
