namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRE050_ONDA")]
    public partial class V_PRE050_ONDA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_ONDA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ONDA { get; set; }

        public short? CD_SITUACAO { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
