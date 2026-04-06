using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
 
    public partial class V_REG090_RAMO_PRODUTO
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
