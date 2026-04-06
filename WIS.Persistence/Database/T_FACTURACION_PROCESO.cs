namespace WIS.Persistence.Database
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FACTURACION_PROCESO")]
    public partial class T_FACTURACION_PROCESO
    {
        public T_FACTURACION_PROCESO()
        {
            this.T_FACTURACION_EJEC_EMPRESA = new HashSet<T_FACTURACION_EJEC_EMPRESA>();
            this.T_FACTURACION_EMPRESA_PROCESO = new HashSet<T_FACTURACION_EMPRESA_PROCESO>();
        }

        [Key]
        [StringLength(20)]
        public string CD_PROCESO { get; set; }
        
        [StringLength(20)]
        public string CD_FACTURACION { get; set; }
        
        [StringLength(30)]
        public string NM_PROCEDIMIENTO { get; set; }
        
        [StringLength(200)]
        public string DS_PROCESO { get; set; }
        
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [StringLength(10)]
        public string NU_CUENTA_CONTABLE { get; set; }
        
        [StringLength(1)]
        public string TP_PROCESO { get; set; }
        
        [StringLength(1)]
        public string FL_EJEC_POR_HORA { get; set; }
        
        public short? CD_SITUACAO_ERROR { get; set; }

        public virtual T_FACTURACION_CODIGO_COMPONEN T_FACTURACION_CODIGO_COMPONEN { get; set; }
        public virtual ICollection<T_FACTURACION_EJEC_EMPRESA> T_FACTURACION_EJEC_EMPRESA { get; set; }
        public virtual ICollection<T_FACTURACION_EMPRESA_PROCESO> T_FACTURACION_EMPRESA_PROCESO { get; set; }
    }
}
