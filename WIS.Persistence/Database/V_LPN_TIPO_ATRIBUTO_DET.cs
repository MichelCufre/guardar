
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_LPN_TIPO_ATRIBUTO_DET")]
    public partial class V_LPN_TIPO_ATRIBUTO_DET
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        [Key]
        [Column(Order = 1)]
        public int ID_ATRIBUTO { get; set; }

        [Required]
        [StringLength(50)]
        public string NM_ATRIBUTO { get; set; }

        [StringLength(400)]
        public string VL_INICIAL { get; set; }

        [StringLength(1)]
        public string FL_REQUERIDO { get; set; }

        [StringLength(1)]
        public string VL_VALIDO_INTERFAZ { get; set; }

        public short? NU_ORDEN { get; set; }

        [StringLength(6)]
        public string ID_ESTADO_INICIAL { get; set; }

        [StringLength(100)]
        public string DS_ESTADO_INICIAL { get; set; }
    }
}
