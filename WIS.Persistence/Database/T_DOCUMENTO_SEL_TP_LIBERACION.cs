namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DOCUMENTO_SEL_TP_LIBERACION")]
    public partial class T_DOCUMENTO_SEL_TP_LIBERACION
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }
    }
}
