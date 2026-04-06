namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_GRUPOS_WEVT030")]
    public partial class V_GRUPOS_WEVT030
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_CONTACTO_GRUPO { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_GRUPO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
