namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ARCHIVO_ADJUNTO_VERSION")]
    public partial class T_ARCHIVO_ADJUNTO_VERSION
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_ARCHIVO_ADJUNTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(5)]
        public string CD_MANEJO { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(200)]
        public string DS_REFERENCIA { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(500)]
        [Column]
        public string LK_RUTA { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_ARCHIVO { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_VERSION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int CD_FUNCIONARIO { get; set; }

        public virtual T_ARCHIVO_ADJUNTO T_ARCHIVO_ADJUNTO { get; set; }
    }
}
