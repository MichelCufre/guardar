namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_LOG_FACTURACION_COR_10")]
    public partial class T_LOG_FACTURACION_COR_10
    {
        [Key]
        public int NU_EJECUCION { get; set; }
        
        [Key]
        public int CD_EMPRESA { get; set; }
        
        [Key]
        [StringLength(20)]
        public string CD_PROCESO { get; set; }
        
        [Key]
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [Key]
        [StringLength(40)]
        public string NU_PALLET { get; set; }
        
        [Key]
        public DateTime DT_FECHA { get; set; }
        
        [Key]
        [StringLength(3)]
        public string TP_RESULTADO { get; set; }
        
        
        [StringLength(3)]
        public string ID_ESTADO { get; set; }
        
        [StringLength(1)]
        public string FL_PARCIAL { get; set; }

        public decimal QT_RESULTADO { get; set; }
        public int? NU_AGENDA { get; set; }
        public DateTime DT_INGRESO { get; set; }
        public DateTime? DT_RETIRO { get; set; }
    }
}
