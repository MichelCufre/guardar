namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FACTURACION_EJEC_EMPRESA")]
    public partial class T_FACTURACION_EJEC_EMPRESA
    {
        [Key]
        public int NU_EJECUCION { get; set; }
        
        [Key]
        public int CD_EMPRESA { get; set; }
        
        [Key]
        [StringLength(20)]
        public string CD_PROCESO { get; set; }
        
        [StringLength(3)]
        public string ID_ESTADO { get; set; }
        
        public short? CD_SITUACAO { get; set; }
        public DateTime? DT_DESDE { get; set; }
        public DateTime? DT_HASTA { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        public virtual T_FACTURACION_PROCESO T_FACTURACION_PROCESO { get; set; }
        public virtual T_FACTURACION_EJECUCION T_FACTURACION_EJECUCION { get; set; }
    }
}
