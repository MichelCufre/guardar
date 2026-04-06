using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_DOC363_DOCUMENTO_INGRESO")]
    public partial class V_DOC363_DOCUMENTO_INGRESO
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        public int? CD_EMPRESA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public long? QT_LINEAS { get; set; }

        public long? QT_PRODUCTO { get; set; }

        public decimal? QT_DOCUMENTO { get; set; }

        public decimal? QT_CIF { get; set; }

        public decimal? QT_AJUSTES { get; set; }

        [StringLength(12)]
        public string VL_FILTRO { get; set; }
    }
}
