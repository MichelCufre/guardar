namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EVENTO_INSTANCIA_EJECUCION")]
    public partial class T_EVENTO_INSTANCIA_EJECUCION
    {
        [Key]
        public int NU_EVENTO_INSTANCIA { get; set; }

        public DateTime? DT_ULT_EJECUCION { get; set; }
    }
}
