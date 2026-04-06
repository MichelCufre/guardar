namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_EVENTO_INTERFAZ_EJECUCION")]
    public partial class V_EVENTO_INTERFAZ_EJECUCION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_INTERFAZ_EXTERNA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ERROR_CARGA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ERROR_PROCEDIMIENTO { get; set; }
    }
}
