namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_CLIENTE_CAMION")]
    public partial class T_CLIENTE_CAMION
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_CAMION { get; set; }
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_CARGA { get; set; }
        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string ID_CARGAR { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_MODALIDAD { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SYNC_REALIZADA { get; set; }
        public virtual T_CAMION T_CAMION { get; set; }
    }
}
