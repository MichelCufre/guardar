namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_TIPO_ENDERECO_WPAR050")]
    public partial class V_TIPO_ENDERECO_WPAR050
    {

        public short? CD_AREA_ARMAZ { get; set; }

        [Key]
        [Required]
        public short CD_TIPO_ENDERECO { get; set; }

        [Required]
        public short CD_TIPO_ESTRUTURA { get; set; }

        [Required]
        [StringLength(30)]
        public string DS_TIPO_ENDERECO { get; set; }

        [StringLength(30)]
        public string DS_TP_ESTRUCTURA { get; set; }

        [Required]
        public DateTime DT_ADDROW { get; set; }

        [Required]
        public DateTime DT_UPDROW { get; set; }

        [StringLength(1)]
        public string ID_VARIOS_LOTES { get; set; }

        [Required]
        [StringLength(1)]
        public string ID_VARIOS_PRODUTOS { get; set; }

        public int? QT_CAPAC_PALETES { get; set; }

        public decimal? QT_VOLUMEN_UNIDAD_FACTURACION { get; set; }

        public decimal? VL_ALTURA { get; set; }

        public decimal? VL_COMPRIMENTO { get; set; }

        public decimal? VL_LARGURA { get; set; }

        public decimal? VL_PESO_MAXIMO { get; set; }

        [StringLength(1)]
        public string FL_RESPETA_CLASE { get; set; }
    }
}
