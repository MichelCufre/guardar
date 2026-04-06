namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_REG605_PALLETS")]
    public partial class V_REG605_PALLETS
    {
        [Key]
        public short CD_PALLET { get; set; }

        [Column]
        [StringLength(60)]
        public string DS_PALLET { get; set; }
        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
