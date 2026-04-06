namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REG140_ONDAS")]
    public partial class V_REG140_ONDAS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_ONDA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ONDA { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        public DateTime? DT_SITUACAO { get; set; }

        public DateTime? DT_ALTERACAO { get; set; }

        public DateTime? DT_CADASTRAMENTO { get; set; }

        [StringLength(1)]
        [Column]
        public string ACTIVO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
        
        [StringLength(100)]
        [Column]
        public string DS_PREDIO { get; set; }
    }
}
