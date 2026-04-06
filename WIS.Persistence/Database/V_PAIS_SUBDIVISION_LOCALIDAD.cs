namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PAIS_SUBDIVISION_LOCALIDAD")]
    public partial class V_PAIS_SUBDIVISION_LOCALIDAD
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long? ID_LOCALIDAD { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_LOCALIDAD { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_LOCALIDAD { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_SUBDIVISION { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_SUBDIVISION { get; set; }

        [Required]
        [StringLength(2)]
        [Column]
        public string CD_PAIS { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_PAIS { get; set; }
    }
}
