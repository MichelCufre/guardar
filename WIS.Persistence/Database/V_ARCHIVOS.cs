
namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_ARCHIVOS")]
    public partial class V_ARCHIVOS
    {
        [Key]
        [StringLength(36)]
        public string CD_ARCHIVO { get; set; }

        [StringLength(100)]
        public string NM_ARCHIVO { get; set; }
        public decimal VL_SIZE { get; set; }
        public DateTime DT_ADDROW { get; set; }
        public int CD_FUNCIONARIO { get; set; }
        public string TP_ENTIDAD { get; set; }
        public string CD_ENTIDAD { get; set; }
    }
}
