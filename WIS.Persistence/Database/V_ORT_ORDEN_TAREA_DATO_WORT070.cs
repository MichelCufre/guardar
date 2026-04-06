using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_ORT_ORDEN_TAREA_DATO_WORT070")]
    public partial class V_ORT_ORDEN_TAREA_DATO_WORT070
    {
        public long NU_ORT_ORDEN_TAREA_DATO { get; set; }

        [StringLength(60)]
        public string DS_TAREA { get; set; }

        [Required]
        [StringLength(10)]
        public string CD_INSUMO_MANIPULEO { get; set; }

        [StringLength(60)]
        public string DS_INSUMO_MANIPULEO { get; set; }

        public decimal QT_INSUMO_MANIPULEO { get; set; }

        [StringLength(100)]
        public string DS_REFERENCIA { get; set; }

        public long NU_ORDEN_TAREA { get; set; }

    }
}
