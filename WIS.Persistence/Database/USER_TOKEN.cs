namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("USER_TOKEN")]
    public partial class USER_TOKEN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int USERID { get; set; }

        public DateTime? DT_CONEXION_USUARIO { get; set; }

        public DateTime? DT_ULTIMO_ENVIO { get; set; }

        [StringLength(20)]
        [Column]
        public string VL_TOKEN { get; set; }

        public virtual USERS USERS { get; set; }
    }
}
