namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DESPACHANTE")]
    public partial class T_DESPACHANTE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_DESPACHANTE { get; set; }

        [StringLength(50)]
        [Column]
        public string NM_DESPACHANTE { get; set; }

        [StringLength(20)]
        [Column]
        public string NU_TELEFONE { get; set; }
    }
}
