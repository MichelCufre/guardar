
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_ATRIBUTO_VALIDACION_ASOCIADO")]
    public partial class V_ATRIBUTO_VALIDACION_ASOCIADO
    {

        [Key]
        [Column(Order = 0)]
        public int ID_ATRIBUTO { get; set; }

        [Key]
        [Column(Order = 1)]
        public short ID_VALIDACION { get; set; }

        [StringLength(50)]
        public string NM_ATRIBUTO { get; set; }

        [StringLength(100)]
        public string VL_ARGUMENTO { get; set; }

        [Required]
        [StringLength(30)]
        public string NM_VALIDACION { get; set; }

        [StringLength(400)]
        public string DS_VALIDACION { get; set; }

        [StringLength(20)]
        public string TP_ARGUMENTO { get; set; }

        [Required]
        [StringLength(10)]
        public string ID_ATRIBUTO_TIPO { get; set; }

        [Required]
        [StringLength(100)]
        public string DS_ATRIBUTO_TIPO { get; set; }

    }
}