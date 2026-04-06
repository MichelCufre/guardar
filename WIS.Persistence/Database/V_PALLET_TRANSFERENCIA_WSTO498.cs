using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_PALLET_TRANSFERENCIA_WSTO498
    {
        [Key]
        [Column(Order = 0)]
        public decimal NU_ETIQUETA { get; set; }
        [Key]
        [Column(Order = 1)]
        public int NU_SEC_ETIQUETA { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        [Column]
        public string CD_ENDERECO_REAL { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CD_ENDERECO_ORIGEN { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal CD_FAIXA { get; set; }
        [Key]
        [Column(Order = 6)]
        public int CD_EMPRESA { get; set; }
        [Key]
        [Column(Order = 3)]
        public string NU_IDENTIFICADOR { get; set; }
        public short? CD_SITUACAO { get; set; }
        public decimal? QT_PRODUTO { get; set; }
        [Column]
        public string ID_AVERIA { get; set; }
        [Column]
        public string ID_INVENTARIO_CICLICO { get; set; }
        [Column]
        public string CD_ENDERECO_DESTINO { get; set; }
        [Column]
        public string CD_ENDERECO_ASIGNADO { get; set; }
        public int? CD_FUNCIONARIO { get; set; }
        [Column]
        public string NM_FUNCIONARIO { get; set; }
        [Column]
        public string LOGINNAME { get; set; }
        [Key]
        [Column(Order = 4)]
        public string CD_PRODUTO { get; set; }
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }
        [Column]
        public string CD_NAM { get; set; }
        [Column]
        public string CD_MERCADOLOGICO { get; set; }
        [Column]
        public string DS_PRODUTO { get; set; }
        [Column]
        public string CD_UNIDADE_MEDIDA { get; set; }
        public int CD_FAMILIA_PRODUTO { get; set; }
        [Column]
        public string CD_CLASSE { get; set; }
        [Column]
        public string DS_REDUZIDA { get; set; }
        [Key]
        [Column(Order = 7)]
        public int NU_SEC_DETALLE { get; set; }
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
