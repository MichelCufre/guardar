namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EVENTO_NOTIFICACION_IMAGEN")]
    public partial class T_EVENTO_NOTIFICACION_IMAGEN
    {
        [Key]
        [StringLength(30)]
        [Column]
        public string CD_IMAGEN { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_IMAGEN { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ARCHIVO { get; set; }
    }
}
