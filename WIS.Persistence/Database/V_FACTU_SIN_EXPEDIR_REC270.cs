namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_FACTU_SIN_EXPEDIR_REC270")]
    public partial class V_FACTU_SIN_EXPEDIR_REC270
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_CARGA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_CAMION { get; set; }
    }
}
