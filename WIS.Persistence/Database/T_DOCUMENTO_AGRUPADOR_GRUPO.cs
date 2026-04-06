using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("T_DOCUMENTO_AGRUPADOR_GRUPO")]
    public partial class T_DOCUMENTO_AGRUPADOR_GRUPO
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(3)]
        public string TP_AGRUPADOR { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        public virtual T_DOCUMENTO_AGRUPADOR_TIPO T_DOCUMENTO_AGRUPADOR_TIPO { get; set; }
    }
}
