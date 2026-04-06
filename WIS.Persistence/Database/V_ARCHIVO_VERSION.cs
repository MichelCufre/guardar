namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_ARCHIVO_VERSION")]
    public partial class V_ARCHIVO_VERSION
    {
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_ARCHIVO_ADJUNTO { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string CD_MANEJO { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(200)]
        public string DS_REFERENCIA { get; set; }

        [Key]
        [Column(Order = 0)]
        public long NU_VERSION{ get; set; }

        [StringLength(500)]
        [Column]
        public string LK_RUTA { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_ARCHIVO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TP_ARCHIVO { get; set; }

        public int CD_FUNCIONARIO { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_FUNCIONARIO { get; set; }
    }
}
