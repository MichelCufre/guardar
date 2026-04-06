namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_REL_LABELESTILO_TIPOCONT")]
    public partial class T_REL_LABELESTILO_TIPOCONT
    {
        public string CD_LABEL_ESTILO { get; set; }

        public string TP_CONTENEDOR { get; set; }

        public T_LABEL_ESTILO T_LABEL_ESTILO { get; set; }

        public T_TIPO_CONTENEDOR T_TIPO_CONTENEDOR { get; set; }
    }
}
