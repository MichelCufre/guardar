namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_SEG020_RECURSOS_ASIGNADOS")]
    public partial class V_SEG020_RECURSOS_ASIGNADOS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PROFILERESOURCEID { get; set; }

        public int? PROFILEID { get; set; }

        public int? RESOURCEID { get; set; }
    }
}
