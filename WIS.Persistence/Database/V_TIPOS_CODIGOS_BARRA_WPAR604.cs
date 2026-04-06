namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_TIPOS_CODIGOS_BARRA_WPAR604")]
    public partial class V_TIPOS_CODIGOS_BARRA_WPAR604
    {
        [Key]
        public int TP_CODIGO_BARRAS { get; set; }
        
        [StringLength(40)]
        public string DS_CODIGO_BARRAS { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public DateTime? DT_ADDROW { get; set; }
    }
}

