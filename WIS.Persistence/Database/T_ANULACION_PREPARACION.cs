using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("T_ANULACION_PREPARACION")]
    public partial class T_ANULACION_PREPARACION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_ANULACION_PREPARACION { get; set; }

        public int NU_PREPARACION { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string ND_ESTADO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ANULACION { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int TP_ANULACION { get; set; }

        [StringLength(50)]
        [Column]
        public string TP_AGRUPACION { get; set; }

        public int? USERID { get; set; }
    }
}
