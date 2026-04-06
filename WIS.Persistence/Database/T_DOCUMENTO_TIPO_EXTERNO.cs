using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("T_DOCUMENTO_TIPO_EXTERNO")]
    public partial class T_DOCUMENTO_TIPO_EXTERNO
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string TP_DOCUMENTO_EXTERNO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [Required]
        [StringLength(50)]
        public string DS_DOCUMENTO_EXTERNO { get; set; }

        public virtual T_DOCUMENTO_TIPO T_DOCUMENTO_TIPO { get; set; }
    }
}
