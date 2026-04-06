using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_INV418_ATRIBUTOS")]
    public partial class V_INV418_ATRIBUTOS
    {
        public decimal NU_INVENTARIO { get; set; }

        public decimal NU_INVENTARIO_ENDERECO { get; set; }

        [Key]
        public decimal NU_INVENTARIO_ENDERECO_DET { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public DateTime? DT_VENCIMIENTO { get; set; }

        public decimal? QT_INVENTARIO { get; set; }

        public decimal? QT_DIFERENCIA { get; set; }

        [StringLength(20)]
        public string ND_ESTADO_INV_ENDERECO_DET { get; set; }

        public long? NU_LPN { get; set; }

        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }

        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        [Key]
        public int ID_ATRIBUTO { get; set; }

        [StringLength(50)]
        public string NM_ATRIBUTO { get; set; }

        [StringLength(400)]
        public string DS_ATRIBUTO { get; set; }

        [StringLength(10)]
        public string ID_ATRIBUTO_TIPO { get; set; }

        [StringLength(400)]
        public string VL_ATRIBUTO { get; set; }
    }
}
