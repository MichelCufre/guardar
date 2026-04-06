namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_RECEPC_AGENDA_REFERENCIA_REL")]
    public partial class T_RECEPC_AGENDA_REFERENCIA_REL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA_REFERENCIA_REL { get; set; }

        public int NU_AGENDA { get; set; }

        public int NU_RECEPCION_REFERENCIA { get; set; }

        public virtual T_AGENDA T_AGENDA { get; set; }

        public virtual T_RECEPCION_REFERENCIA T_RECEPCION_REFERENCIA { get; set; }
    }
}
