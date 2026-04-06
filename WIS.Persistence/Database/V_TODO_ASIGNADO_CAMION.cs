namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_TODO_ASIGNADO_CAMION")]
    public partial class V_TODO_ASIGNADO_CAMION
    {
        [Column(Order = 0)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Column(Order = 1)]
        public int NU_SEQ_PREPARACION { get; set; }

        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Column(Order = 3)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        public int? CD_CAMION { get; set; }

	}
}
