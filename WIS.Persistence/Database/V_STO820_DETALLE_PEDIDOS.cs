using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STO820_DETALLE_PEDIDOS")]
    public partial class V_STO820_DETALLE_PEDIDOS
    {
        public long NU_TRASPASO { get; set; }

        [Required]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Required]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(40)]
        public string NU_PEDIDO_DESTINO { get; set; }

        [StringLength(10)]
        public string CD_CLIENTE_DESTINO { get; set; }

        public int CD_EMPRESA_DESTINO { get; set; }

        [Required]
        [StringLength(6)]
        public string TP_PEDIDO_DESTINO { get; set; }

        [Required]
        [StringLength(6)]
        public string TP_EXPEDICION_DESTINO { get; set; }

        [StringLength(6)]
        public string TP_PEDIDO { get; set; }

        [StringLength(6)]
        public string TP_EXPEDICION { get; set; }

    }
}
