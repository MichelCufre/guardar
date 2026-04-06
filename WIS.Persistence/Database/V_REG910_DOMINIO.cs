namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_REG910_DOMINIO")]
    public partial class V_REG910_DOMINIO
    {
        [Key]
        [StringLength(10)]
        [Column]
        public string CD_DOMINIO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO { get; set; }

        [StringLength(1)]
        [Column]

        public string FL_INTERNO_WIS { get; set; }
    }
}
