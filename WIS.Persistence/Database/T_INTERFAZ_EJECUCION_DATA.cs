namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_INTERFAZ_EJECUCION_DATA")]
    public partial class T_INTERFAZ_EJECUCION_DATA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public byte[] DATA { get; set; }

        public virtual T_INTERFAZ_EJECUCION T_INTERFAZ_EJECUCION { get; set; }
    }
}
