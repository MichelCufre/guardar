namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FACTURACION_EJECUCION")]
    public partial class T_FACTURACION_EJECUCION
    {
        public T_FACTURACION_EJECUCION()
        {
            this.T_FACTURACION_EJEC_EMPRESA = new HashSet<T_FACTURACION_EJEC_EMPRESA>();
            this.T_FACTURACION_RESULTADO = new HashSet<T_FACTURACION_RESULTADO>();
        }

        [Key]
        public int NU_EJECUCION { get; set; }

        [StringLength(100)]
        public string NM_EJECUCION { get; set; }
        
        [StringLength(1)]
        public string FL_EJEC_POR_HORA { get; set; }
        
        public DateTime? DT_DESDE { get; set; }
        public DateTime? DT_HASTA { get; set; }
        public DateTime? DT_CORTE_QUINCENA { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_PROGRAMACION { get; set; }
        public DateTime? DT_EJECUCION { get; set; }
        public DateTime? DT_APROBACION { get; set; }
        public DateTime? DT_ENVIADA { get; set; }
        public DateTime? DT_ANULACION { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public int? CD_FUNC_EJECUCION { get; set; }
        public int? CD_FUNC_PROGRAMACION { get; set; }
        public int? CD_FUNC_APROBACION { get; set; }
        public int? CD_FUNC_ANULACION { get; set; }
        public int? CD_FUNC_ENVIADA { get; set; }
        public short? CD_SITUACAO { get; set; }

        public virtual ICollection<T_FACTURACION_EJEC_EMPRESA> T_FACTURACION_EJEC_EMPRESA { get; set; }
        public virtual ICollection<T_FACTURACION_RESULTADO> T_FACTURACION_RESULTADO { get; set; }
    }
}
