using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_ESTOQUE_STO005
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }
        [Key]
        [Column(Order = 1)]
        public int CD_EMPRESA { get; set; }
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CD_PRODUTO { get; set; }
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }
        [StringLength(2)]
        [Column]
        public string CD_CLASSE { get; set; }
        [StringLength(35)]
        [Column]
        public string DS_CLASSE { get; set; }
        public int CD_FAMILIA_PRODUTO { get; set; }
        [StringLength(30)]
        [Column]
        public string DS_FAMILIA_PRODUTO { get; set; }
        public short CD_SITUACAO { get; set; }
        [StringLength(20)]
        [Column]
        public string CD_MERCADOLOGICO { get; set; }
        public short? CD_ROTATIVIDADE { get; set; }
        [StringLength(20)]
        [Column]
        public string DS_REDUZIDA { get; set; }
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }
        public DateTime? DT_FABRICACAO { get; set; }
        public DateTime? DT_INVENTARIO { get; set; }
        public decimal? QT_ESTOQUE { get; set; }
        public decimal? QT_RESERVA_SAIDA { get; set; }
        public decimal? QT_TRANSITO_ENTRADA { get; set; }
        [Key]
        [Column(Order = 4)]
        public decimal CD_FAIXA { get; set; }
        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }
        [StringLength(1)]
        [Column]
        public string ID_CTRL_CALIDAD { get; set; }
        [StringLength(1)]
        [Column]
        public string ID_AVERIA { get; set; }
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
