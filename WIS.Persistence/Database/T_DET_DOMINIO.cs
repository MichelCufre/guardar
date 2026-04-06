namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DET_DOMINIO")]
    public partial class T_DET_DOMINIO
    {
        [Key]
        [StringLength(50)]
        [Column]
        public string NU_DOMINIO { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string CD_DOMINIO { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string CD_DOMINIO_VALOR { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO_VALOR { get; set; }
        public virtual T_DOMINIO T_DOMINIO { get; set; }
    }
}
