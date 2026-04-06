using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_DOCUMENTOS_ACTA_DOC310")]
    public partial class V_DOCUMENTOS_ACTA_DOC310
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string DOCUMENTO_ACTA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TIPO_DOCUMENTO_ACTA { get; set; }

        public DateTime? FECHA_AGREGADO_ACTA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(22)]
        public string DOCUMENTO_AFECTADO { get; set; }


        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string NU_DOCUMENTO_AFECTADO { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(6)]
        public string TIPO_DOCUMENTO_AFECTADO { get; set; }

        public DateTime? FECHA_AGREGADO_DOC_AFECTADO { get; set; }

        [StringLength(25)]
        public string DUA_DOCUMENTO_AFECTADO { get; set; }

        [StringLength(24)]
        public string TIPO_DUA_DOCUMENTO_AFECTADO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }
    }
}
