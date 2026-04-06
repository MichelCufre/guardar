using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{
    [Table("T_LPN_DET")]
    public partial class T_LPN_DET
    {
        [Key]
        [Column(Order = 0)]
        public int ID_LPN_DET { get; set; }

        [Key]
        [Column(Order = 1)]
        public long NU_LPN { get; set; }    

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 4)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal QT_ESTOQUE { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public decimal? QT_DECLARADA { get; set; }

        public decimal? QT_RESERVA_SAIDA { get; set; }

        public decimal? QT_EXPEDIDA { get; set; }

        [StringLength(40)]
        [Column]
        public string ID_LINEA_SISTEMA_EXTERNO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AVERIA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_INVENTARIO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_CTRL_CALIDAD { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_MOTIVO_AVERIA { get; set; }

        public virtual T_LPN T_LPN { get; set; }
    }
}
