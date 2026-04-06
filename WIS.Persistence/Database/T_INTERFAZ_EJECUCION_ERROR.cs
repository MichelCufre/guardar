namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_INTERFAZ_EJECUCION_ERROR")]
    public partial class T_INTERFAZ_EJECUCION_ERROR
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
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

        public virtual T_INTERFAZ_EJECUCION T_INTERFAZ_EJECUCION { get; set; }
    }
}
