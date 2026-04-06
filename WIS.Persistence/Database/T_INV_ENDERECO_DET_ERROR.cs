namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_INV_ENDERECO_DET_ERROR")]
    public partial class T_INV_ENDERECO_DET_ERROR
    {
        [Key]
        public decimal NU_ERROR { get; set; }

        public decimal? NU_INVENTARIO { get; set; }

        public decimal? NU_INVENTARIO_ENDERECO_DET { get; set; }

        [StringLength(400)]
        [Column]
        public string DS_ERROR { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
