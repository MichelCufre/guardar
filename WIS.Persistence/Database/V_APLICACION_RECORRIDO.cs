
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;

namespace WIS.Persistence.Database
{

    [Table("V_APLICACION_RECORRIDO")]
    public partial class V_APLICACION_RECORRIDO
    {

        [Key]
        public int NU_RECORRIDO { get; set; }

        [Required]
        [StringLength(50)]
        public string NM_RECORRIDO { get; set; }

        [Required]
        [StringLength(200)]
        public string DS_RECORRIDO { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [Key]
        [StringLength(30)]
        public string CD_APLICACION { get; set; }

        [StringLength(100)]
        public string DS_APLICACION { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_PREDETERMINADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

    }
}
