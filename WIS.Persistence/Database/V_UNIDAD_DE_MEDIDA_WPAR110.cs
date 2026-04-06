
namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_UNIDAD_DE_MEDIDA_WPAR110")]
    public partial class V_UNIDAD_DE_MEDIDA_WPAR110
    {
        [Key]
        [StringLength(10)]
        public string CD_UNIDADE_MEDIDA { get; set; }
        
        [StringLength(30)]
        public string DS_UNIDADE_MEDIDA { get; set; }
        
        [StringLength(1)]
        public string FG_ACEITA_DECIMAL { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(10)]
        public string CD_UNIDAD_MEDIDA_EXTERNA { get; set; }
    }
}
