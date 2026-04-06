namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FACTURACION_UNIDAD_MEDIDA")]
    public class T_FACTURACION_UNIDAD_MEDIDA
    {
        [Key]
        [StringLength(10)]
        public string CD_UNIDADE_MEDIDA { get; set; }
        
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [StringLength(1)]
        public string ID_USO { get; set; }

        public long? NU_TRANSACCION { get; set; }
    }
}
