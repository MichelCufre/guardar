namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRE051_PEDIDOS_COMPATIBLES")]
    public partial class V_PRE051_PEDIDOS_COMPATIBLES
    {
        [Key]
        [StringLength(40)]
        [Column(Order = 0)]
        public string NU_PEDIDO_AUTO { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string CD_CLIENTE_AUTO { get; set; }

        [Key]
        [StringLength(40)]
        [Column(Order = 1)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [StringLength(40)]
        [Column(Order = 2)]
        public string NU_PEDIDO_ESPE { get; set; }

        [Key]
        [StringLength(10)]
        [Column(Order = 3)]
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
