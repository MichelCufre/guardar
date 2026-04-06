namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FACTURACION_EMPRESA_PROCESO")]
    public partial class T_FACTURACION_EMPRESA_PROCESO
    {
        [Key]
        public int CD_EMPRESA { get; set; }
        
        [Key]
        [StringLength(20)]
        public string CD_PROCESO { get; set; }
        
        [StringLength(1)]
        public string TP_ULTIMO_PROCESO { get; set; }
        
        public short? CD_SITUACAO_ERROR { get; set; }

        public decimal? QT_RESULTADO { get; set; }

        public DateTime? HR_ULTIMO_PROCESO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public T_FACTURACION_PROCESO T_FACTURACION_PROCESO { get; set; }
    }
}
