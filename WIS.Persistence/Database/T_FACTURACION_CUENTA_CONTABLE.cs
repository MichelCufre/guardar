namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FACTURACION_CUENTA_CONTABLE")]
    public partial class T_FACTURACION_CUENTA_CONTABLE
    {
        public T_FACTURACION_CUENTA_CONTABLE()
        {
            this.T_FACTURACION_CODIGO_COMPONEN = new HashSet<T_FACTURACION_CODIGO_COMPONEN>();
            this.T_FACTURACION_RESULTADO = new HashSet<T_FACTURACION_RESULTADO>();
        }

        [Key]
        [StringLength(10)]
        public string NU_CUENTA_CONTABLE { get; set; }

        [StringLength(100)]
        public string DS_CUENTA_CONTABLE { get; set; }

        public virtual ICollection<T_FACTURACION_CODIGO_COMPONEN> T_FACTURACION_CODIGO_COMPONEN { get; set; }
        public virtual ICollection<T_FACTURACION_RESULTADO> T_FACTURACION_RESULTADO { get; set; }
    }
}
