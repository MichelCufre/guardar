namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FACTURACION_PALLET_DET")]
    public partial class T_FACTURACION_PALLET_DET
    {
        [Key]
        public int NU_PALLET_DET { get; set; }
        
        [StringLength(40)]
        public string NU_PALLET { get; set; }
        
        [StringLength(20)]
        public string CD_FACTURACION { get; set; }
        
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [StringLength(5)]
        public string ID_ESTADO { get; set; }
        
        [StringLength(1)]
        public string FL_APLICO_MINIMO { get; set; }
        
        public int? NU_EJECUCION_FACTURACION { get; set; }
        public int? CD_EMPRESA { get; set; }
        public int? NU_BULTOS_FACTURADOS { get; set; }
        public decimal? QT_RESULTADO { get; set; }
        public DateTime? DT_DESDE { get; set; }
        public DateTime? DT_HASTA { get; set; }

        public virtual T_FACTURACION_PALLET T_FACTURACION_PALLET { get; set; }
    }
}
