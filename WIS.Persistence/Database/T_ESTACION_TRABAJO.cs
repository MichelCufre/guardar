namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ESTACION_TRABAJO")]
    public partial class T_ESTACION_TRABAJO
    {
        [Key]
        [StringLength(20)]
        [Column]
        public string CD_ESTACION { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_ESTACION { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_LABEL_ESTILO_DEFAULT { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_IMPRESORA_DEFAULT { get; set; }
    }
}
