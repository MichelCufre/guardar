namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FACTURACION_CODIGO")]
    public partial class T_FACTURACION_CODIGO
    {
        public T_FACTURACION_CODIGO()
        {
            this.T_FACTURACION_CODIGO_COMPONEN = new HashSet<T_FACTURACION_CODIGO_COMPONEN>();
        }

        [Key]
        [StringLength(20)]
        public string CD_FACTURACION { get; set; }
        
        [StringLength(100)]
        public string DS_FACTURACION { get; set; }
        
        [StringLength(1)]
        public string TP_CALCULO { get; set; }
        
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        public virtual ICollection<T_FACTURACION_CODIGO_COMPONEN> T_FACTURACION_CODIGO_COMPONEN { get; set; }
    }
}
