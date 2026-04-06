namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EVENTO_NOTIFICACION_ARCHIVO")]
    public partial class T_EVENTO_NOTIFICACION_ARCHIVO
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_EVENTO_NOTIFICACION_ARCHIVO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_EVENTO_NOTIFICACION { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ARCHIVO { get; set; }

        [StringLength(100)]
        [Column]
        public string ID_REFERENCIA { get; set; }

        [StringLength(30)]
        [Column]
        public string TP_REFERENCIA { get; set; }

        public byte[] VL_DATA { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public virtual T_EVENTO_NOTIFICACION T_EVENTO_NOTIFICACION { get; set; }
    }
}
