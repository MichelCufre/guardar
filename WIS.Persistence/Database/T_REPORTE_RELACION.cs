namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_REPORTE_RELACION")]
    public partial class T_REPORTE_RELACION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_REPORTE_RELACION { get; set; }

        public long NU_REPORTE { get; set; }

        [Required]
        [StringLength(200)]
        [Column]
        public string CD_CLAVE { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string NM_TABLA { get; set; }

        public virtual T_REPORTE T_REPORTE { get; set; }
    }
}
