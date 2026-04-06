using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("T_ANULACION_PREPARACION_ERROR")]
    public partial class T_ANULACION_PREPARACION_ERROR
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_ANULACION_PREPARACION_ERROR { get; set; }

        public int NU_ANULACION_PREPARACION { get; set; }

        [StringLength(2000)]
        [Column]
        public string DS_ERROR { get; set; }
    }
}
