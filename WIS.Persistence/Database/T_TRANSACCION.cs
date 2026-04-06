namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_TRANSACCION")]
    public partial class T_TRANSACCION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_TRANSACCION { get; set; }

        [StringLength(500)]
        [Column]
        public string DS_TRANSACCION { get; set; }

        [StringLength(200)]
        [Column]
        public string CD_APLICACION { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_DATA { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_ADDROW_FIN_TRAN { get; set; }
    }
}
