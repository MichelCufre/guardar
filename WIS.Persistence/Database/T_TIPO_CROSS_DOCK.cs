namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_TIPO_CROSS_DOCK")]
    public partial class T_TIPO_CROSS_DOCK
    {
        [Key]
        [StringLength(3)]
        public string CD_TIPO_CROSS_DOCK { get; set; }

        [Required]
        [StringLength(50)]
        public string DS_TIPO_CROSS_DOCK { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(1)]
        public string FL_REQUIERE_CIERRE_AGENDA { get; set; }

        [StringLength(1)]
        public string FL_REQUIERE_LIBERACION_AGENDA { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_ACTIVO { get; set; }
    }
}
