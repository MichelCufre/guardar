namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FACTURACION_RESULTADO")]
    public partial class T_FACTURACION_RESULTADO
    {
        [Key]
        public int NU_EJECUCION { get; set; }
        
        [Key]
        public int CD_EMPRESA { get; set; }
        
        [Key]
        [StringLength(20)]
        public string CD_FACTURACION { get; set; }
        
        [Key]
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [StringLength(10)]
        public string CD_UNIDADE_MEDIDA { get; set; }
        
        [StringLength(20)]
        public string NU_FACTURA { get; set; }
        
        [StringLength(10)]
        public string NU_CUENTA_CONTABLE { get; set; }
        
        [StringLength(1000)]
        public string DS_ADICIONAL { get; set; }
        
        [StringLength(20)]
        public string CD_PROCESO { get; set; }
        
        [StringLength(20)]
        public string NU_TICKET_INTERFAZ_FACTURACION { get; set; }
        
        public decimal? QT_RESULTADO { get; set; }

        public short? CD_SITUACAO { get; set; }

        public int? CD_FUNC_APROBACION_RECHAZO { get; set; }

        public DateTime? DT_APROBACION_RECHAZO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? VL_PRECIO_MINIMO { get; set; }

        public decimal? VL_PRECIO_UNITARIO { get; set; }

        [StringLength(15)]
        public string CD_MONEDA { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public virtual T_FACTURACION_CUENTA_CONTABLE T_FACTURACION_CUENTA_CONTABLE { get; set; }
        
        public virtual T_FACTURACION_EJECUCION T_FACTURACION_EJECUCION { get; set; }
    }
}
