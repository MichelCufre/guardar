namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_INT070_INT_EJECUCION_ERROR")]
    public partial class V_INT070_INT_EJECUCION_ERROR
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }
        [Key]
        public int NU_ERROR { get; set; }

        public int? NU_REGISTRO { get; set; }

        [StringLength(400)]
        [Column]
        public string DS_REFERENCIA { get; set; }

        public int? CD_PARAMETRO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_ERROR { get; set; }

        [StringLength(400)]
        [Column]
        public string DS_ERROR { get; set; }
    }
}
