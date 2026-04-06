using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_DOCUMENTO_MOV_MERC_DOC350")]
    public partial class V_DOCUMENTO_MOV_MERC_DOC350
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string NU_AGRUPADOR { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(3)]
        public string TP_AGRUPADOR { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(62)]
        public string NU_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        public decimal? QT_VOLUMEN { get; set; }

        public decimal? QT_PESO_BRUTO { get; set; }

        public decimal? QT_PESO_LIQUIDO { get; set; }

        public decimal? VL_DOCUMENTO { get; set; }

        public decimal? VL_DOCUMENTO_CIF { get; set; }

        public decimal? VL_TRIBUTO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }

}
