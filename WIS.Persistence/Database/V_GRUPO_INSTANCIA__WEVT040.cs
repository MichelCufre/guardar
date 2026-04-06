namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_GRUPO_INSTANCIA__WEVT040")]
    public partial class V_GRUPO_INSTANCIA__WEVT040
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_CONTACTO_GRUPO { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_GRUPO { get; set; }

        public int? NU_EVENTO_INSTANCIA { get; set; }
    }
}
