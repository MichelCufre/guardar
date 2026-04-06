namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_CARGA")]
    public partial class T_CARGA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_CARGA { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_CARGA { get; set; }

        public short? CD_ROTA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public int? NU_PREPARACION { get; set; }
    }
}
