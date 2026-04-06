
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("T_ATRIBUTO_VALIDACION")]
    public partial class T_ATRIBUTO_VALIDACION
    {

        [Key]
        [Column(Order = 0)]
        public short ID_VALIDACION { get; set; }

        [Required]
        [StringLength(30)]
        public string NM_VALIDACION { get; set; }

        [StringLength(400)]
        public string DS_VALIDACION { get; set; }

        [Required]
        [StringLength(10)]
        public string ID_ATRIBUTO_TIPO { get; set; }

        [StringLength(100)]
        public string NM_ARGUMENTO { get; set; }

        [StringLength(20)]
        public string TP_ARGUMENTO { get; set; }

        [StringLength(100)]
        public string DS_ERROR { get; set; }

    }
}