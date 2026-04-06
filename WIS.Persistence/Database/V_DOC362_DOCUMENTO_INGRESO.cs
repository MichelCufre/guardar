using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_DOC362_DOCUMENTO_INGRESO")]
    public partial class V_DOC362_DOCUMENTO_INGRESO
    {
        public DateTime? DT_ADDROW { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [StringLength(100)]
        public string DS_DOCUMENTO { get; set; }

        public DateTime? DT_PROGRAMADO { get; set; }

        [StringLength(20)]
        public string NU_AGRUPADOR { get; set; }

        [StringLength(3)]
        public string TP_AGRUPADOR { get; set; }

        [StringLength(20)]
        public string NU_LACRE { get; set; }

        public int? NU_AGENDA { get; set; }

        [StringLength(200)]
        public string NU_FACTURA { get; set; }

        public DateTime? DT_FINALIZADO { get; set; }

        public long? QT_LINEAS { get; set; }

        public long? QT_PRODUCTO { get; set; }

        public decimal? QT_DOCUMENTO { get; set; }

        public decimal? QT_CIF { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(6)]
        public string ID_ESTADO { get; set; }

        [StringLength(100)]
        public string DS_ESTADO { get; set; }
    }
}
