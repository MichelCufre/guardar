namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FACTURACION_CODIGO_COMPONEN")]
    public partial class T_FACTURACION_CODIGO_COMPONEN
    {
        public T_FACTURACION_CODIGO_COMPONEN()
        {
            this.T_FACTURACION_LISTA_COTIZACION = new HashSet<T_FACTURACION_LISTA_COTIZACION>();
            this.T_FACTURACION_PROCESO = new HashSet<T_FACTURACION_PROCESO>();
        }

        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CD_FACTURACION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_SIGNIFICADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_CUENTA_CONTABLE { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public virtual ICollection<T_FACTURACION_LISTA_COTIZACION> T_FACTURACION_LISTA_COTIZACION { get; set; }
        public virtual ICollection<T_FACTURACION_PROCESO> T_FACTURACION_PROCESO { get; set; }
        public virtual T_FACTURACION_CUENTA_CONTABLE T_FACTURACION_CUENTA_CONTABLE { get; set; }
        public virtual T_FACTURACION_CODIGO T_FACTURACION_CODIGO { get; set; }
    }
}
