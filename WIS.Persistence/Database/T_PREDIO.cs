namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PREDIO")]
    public partial class T_PREDIO
    {
        [Key]
        [StringLength(10)]
        [Column(Order = 0)]
        public string NU_PREDIO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_PREDIO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ENDERECO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_PUNTO_ENTREGA { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SYNC_REALIZADA { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_EXTERNO { get; set; }
    }
}
