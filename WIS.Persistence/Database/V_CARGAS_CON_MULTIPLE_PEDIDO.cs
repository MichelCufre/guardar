namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_CARGAS_CON_MULTIPLE_PEDIDO")]
    public partial class V_CARGAS_CON_MULTIPLE_PEDIDO
    {
        [Key]
        public long? NU_CARGA { get; set; }
    }
}
