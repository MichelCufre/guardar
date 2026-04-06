namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PEDIDOS_COMPATIBLES_WPRE051")]
    public partial class V_PEDIDOS_COMPATIBLES_WPRE051
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string NU_PEDIDO_AUTO { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string CD_CLIENTE_AUTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string NU_PEDIDO_ESPE { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string CD_CLIENTE_ESPE { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA_ESPECIFICADO { get; set; }

        public short? CD_ONDA { get; set; }

        public short? CD_ONDA_ESPECIFICADO { get; set; }
    }
}
