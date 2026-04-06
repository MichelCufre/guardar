namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PREDIO_USUARIO")]
    public partial class T_PREDIO_USUARIO
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string NU_PREDIO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int USERID { get; set; }
    }
}
