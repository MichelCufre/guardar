namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("T_PORTERIA_VEHICULO_OBJETO")]
    public partial class T_PORTERIA_VEHICULO_OBJETO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PORTERIA_VEHICULO_OBJETO { get; set; }

        public int NU_PORTERIA_VEHICULO { get; set; }

        [Required]
        [StringLength(50)]
        [Column]
        public string CD_OBJETO { get; set; }

        [Required]
        [StringLength(5)]
        [Column]
        public string TP_OBJETO { get; set; }
    }
}
