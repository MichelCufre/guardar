namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_CONTENEDORES_PREDEFINIDOS")]
    public partial class T_CONTENEDORES_PREDEFINIDOS
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_CONTENEDOR { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string TP_CONTENEDOR { get; set; }

        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_EXTERNO_CONTENEDOR { get; set; }
    }
}
