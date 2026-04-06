namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REG009_PRODUCTO_COMPONENTE1")]
    public partial class V_REG009_PRODUCTO_COMPONENTE1
    {
        [Key]
        [StringLength(20)]
        [Column]
        public string ND_FACTURACION_COMP1 { get; set; }
    }
}
