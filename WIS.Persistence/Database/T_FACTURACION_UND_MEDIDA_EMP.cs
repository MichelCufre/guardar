namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FACTURACION_UND_MEDIDA_EMP")]
    public class T_FACTURACION_UND_MEDIDA_EMP
    {
        [Key]
        [StringLength(10)]
        public string CD_UNIDADE_MEDIDA { get; set; }
        [Key]
        public int CD_EMPRESA { get; set; }
        
        public int? CD_FUNCIONARIO { get; set; }
        public DateTime? DT_ADDROW { get; set; }

        public virtual T_FACTURACION_UNIDAD_MEDIDA T_FACTURACION_UNIDAD_MEDIDA { get; set; }
    }
}
