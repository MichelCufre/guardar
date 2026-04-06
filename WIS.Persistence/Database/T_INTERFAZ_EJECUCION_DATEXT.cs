namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_INTERFAZ_EJECUCION_DATEXT")]
    public partial class T_INTERFAZ_EJECUCION_DATEXT
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        public int CD_EMPRESA { get; set; }

        public short NU_TOTAL_PAQUETES { get; set; }

        [StringLength(1)]
        [Column]
        public string FINALIZA_EJECUCION { get; set; }

        public virtual T_INTERFAZ_EJECUCION T_INTERFAZ_EJECUCION { get; set; }
    }
}
