
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_LPN_TIPO_ATRIBUTO_DET")]
    public partial class T_LPN_TIPO_ATRIBUTO_DET
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        [Key]
        [Column(Order = 1)]
        public int ID_ATRIBUTO { get; set; }

        [StringLength(400)]
        public string VL_INICIAL { get; set; }

        [StringLength(1)]
        public string FL_REQUERIDO { get; set; }

        [StringLength(1)]
        public string VL_VALIDO_INTERFAZ { get; set; }

        public short? NU_ORDEN { get; set; }

        [StringLength(6)]
        public string ID_ESTADO_INICIAL { get; set; }
    }
}
