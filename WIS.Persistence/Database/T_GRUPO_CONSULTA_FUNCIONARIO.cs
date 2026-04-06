namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_GRUPO_CONSULTA_FUNCIONARIO")]
    public partial class T_GRUPO_CONSULTA_FUNCIONARIO
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int USERID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string CD_GRUPO_CONSULTA { get; set; }

        public virtual T_GRUPO_CONSULTA T_GRUPO_CONSULTA { get; set; }
        public virtual USERS USERS { get; set; }
    }
}
