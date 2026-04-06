using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("T_RAMO_PRODUTO")]
    public partial class T_RAMO_PRODUTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_RAMO_PRODUTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_RAMO_PRODUTO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_ADDROW { get; set; }

    }
}
