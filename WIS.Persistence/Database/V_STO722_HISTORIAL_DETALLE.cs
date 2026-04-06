using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STO722_HISTORIAL_DETALLE")]
    public partial class V_STO722_HISTORIAL_DETALLE
    {
        [Key]
        public long NU_LOG_SECUENCIA { get; set; }

        public DateTime? DT_LOG_ADD_ROW { get; set; }

        public int? CD_LOG_USERID { get; set; }

        [StringLength(100)]
        public string FULLNAME { get; set; }

        [StringLength(1)]
        public string ID_LOG_TRIGGER { get; set; }

        public int ID_LPN_DET { get; set; }

        public long NU_LPN { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal QT_ESTOQUE { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        public decimal? QT_DECLARADA { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public decimal? QT_RESERVA_SAIDA { get; set; }

        [StringLength(30)]
        public string CD_LOG_APLICACION { get; set; }

        [StringLength(100)]
        public string DS_APLICACAO { get; set; }

        [StringLength(500)]
        public string DS_TRANSACCION { get; set; }
    }
}