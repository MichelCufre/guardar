using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_DOCUMENTO_AGRUPABLE_DOC330")]
    public partial class V_DOCUMENTO_AGRUPABLE_DOC330
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [StringLength(100)]
        public string DS_DOCUMENTO { get; set; }

        public int? CD_EMPRESA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public int? NU_AGENDA { get; set; }

        [StringLength(6)]
        public string ID_ESTADO { get; set; }

        [StringLength(100)]
        public string DS_ESTADO { get; set; }

        [StringLength(20)]
        public string NU_AGRUPADOR { get; set; }

        [StringLength(3)]
        public string TP_AGRUPADOR { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(100)]
        public string DS_CLIENTE { get; set; }
    }
}
