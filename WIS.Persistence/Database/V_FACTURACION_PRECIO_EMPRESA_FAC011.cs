namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_FACTURACION_PRECIO_EMPRESA_FAC011")]
    public class V_FACTURACION_PRECIO_EMPRESA_FAC011
    {
        [Key]
        public int CD_EMPRESA { get; set; }

        public int? CD_LISTA_PRECIO { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }
        
        [StringLength(50)]
        public string DS_LISTA_PRECIO  { get; set; }
        
        [StringLength(15)]
        public string CD_MONEDA { get; set; }

    }
}
