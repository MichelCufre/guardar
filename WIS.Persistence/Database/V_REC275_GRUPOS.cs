
namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_REC275_GRUPOS")]
    public partial class V_REC275_GRUPOS
    {
        [Key]
        [StringLength(50)]
        [Column]
        public string CD_GRUPO { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_GRUPO { get; set; }

    }
}
