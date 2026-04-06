using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_INV412_DET_CONTEO")]
    public partial class V_INV412_DET_CONTEO
    {
        [Key]
        public decimal NU_INVENTARIO_ENDERECO_DET { get; set; }

        public decimal NU_INVENTARIO_ENDERECO { get; set; }

        public decimal NU_INVENTARIO { get; set; }

        [StringLength(200)]
        public string DS_INVENTARIO { get; set; }

        [StringLength(9)]
        public string TP_INVENTARIO { get; set; }

        [StringLength(10)]
        public string DS_TP_INVENTARIO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ESTADO_INVENTARIO { get; set; }

        [StringLength(100)]
        public string DS_ESTADO_INVENTARIO { get; set; }

        public decimal? NU_CONTEO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        public decimal? CD_FAIXA { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public DateTime? DT_VENCIMIENTO { get; set; }

        [StringLength(20)]
        public string ND_ESTADO_INV_ENDERECO_DET { get; set; }

        [StringLength(100)]
        public string DS_ESTADO_INV_ENDERECO_DET { get; set; }

        public decimal? QT_INVENTARIO { get; set; }

        public decimal? QT_STOCK { get; set; }

        public decimal? QT_DIFERENCIA { get; set; }

        public decimal? QT_RESERVA_SAIDA { get; set; }

        public long? NU_LPN { get; set; }

        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }

        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        public int? ID_LPN_DET { get; set; }

        [StringLength(40)]
        public string ID_LINEA_SISTEMA_EXTERNO { get; set; }

        [StringLength(3)]
        public string CD_MOTIVO_AJUSTE { get; set; }

        [StringLength(60)]
        public string DS_MOTIVO_AJUSTE { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [StringLength(2)]
        public string CD_CLASSE { get; set; }

        [StringLength(100)]
        public string DS_CLASSE { get; set; }

        public int? CD_FAMILIA_PRODUTO { get; set; }

        [StringLength(100)]
        public string DS_FAMILIA_PRODUTO { get; set; }

        public short? CD_RAMO_PRODUTO { get; set; }

        [StringLength(200)]
        public string DS_RAMO_PRODUTO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public int? CD_USUARIO { get; set; }

        [StringLength(100)]
        public string FULLNAME { get; set; }

        [StringLength(50)]
        public string LOGINNAME { get; set; }

        public decimal? QT_TIEMPO_INSUMIDO { get; set; }

        [StringLength(1)]
        public string FL_CONTEO_ESPERADO { get; set; }
    }
}
