using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WIS.Persistence.Database
{
    [Table("V_REC171_AGENDA_DETALLE")]
    public partial class V_REC171_AGENDA_DETALLE
    {
		public int QT_ETIQUETA_IMPRIMIR { get; set; }

        [Key]
        [Column(Order = 0)]
        public int NU_AGENDA { get; set; }

        [Key]
        [Column(Order = 1)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal CD_FAIXA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        public decimal? QT_AGENDADO_ORIGINAL { get; set; }

        public decimal? QT_AGENDADO { get; set; }

        public decimal? PESO_AGENDADO { get; set; }

        public decimal? VOLUMEN_AGENDADO { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public decimal? PESO_RECIBIDO { get; set; }

        public decimal? VOLUMEN_RECIBIDO { get; set; }

        public long? CANT_ETIQUETAS { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(30)]
        public string DS_SITUACAO { get; set; }

        public DateTime? DT_ENTRADA { get; set; }

        public DateTime? DT_ACEPTADA_RECEPCION { get; set; }

        public int? CD_FUNC_ACEPTO_RECEPCION { get; set; }

        [StringLength(30)]
        public string NM_FUNCIONARIO { get; set; }

        [StringLength(1)]
        public string TP_BARRAS_DISTINTO_CERO { get; set; }

        public decimal? QT_UNIDADES_BULTO { get; set; }

        [StringLength(2)]
        public string CD_CLASSE { get; set; }

        [StringLength(100)]
        public string DS_CLASSE { get; set; }

        [StringLength(100)]
        public string DS_FAMILIA_PRODUTO { get; set; }

        [StringLength(30)]
        public string CD_UNIDADE_MEDIDA { get; set; }

        public decimal? QT_UND_BULTO { get; set; }

        public decimal? QT_BULTOS_RECIBIDO { get; set; }

        public decimal? QT_BULTOS_AGENDADO { get; set; }

        public decimal? VL_PRECO_VENDA { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        public short? CD_RAMO_PRODUTO { get; set; }

        [StringLength(200)]
        public string DS_RAMO_PRODUTO { get; set; }

        [StringLength(200)]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO4 { get; set; }

        [StringLength(18)]
        public string DS_ANEXO5 { get; set; }

        public int? CD_FAMILIA_PRODUTO { get; set; }

        public decimal? QT_GENERICO { get; set; }

        public decimal? QT_ALMACENADO { get; set; }

        public decimal? QT_CROSS_DOCK_ASIGNADO { get; set; }

        public decimal? QT_CROSS_DOCK_PREPARADO { get; set; }

        public DateTime? DT_VENCIMIENTO { get; set; }

        [StringLength(30)]
        public string NU_DOCUMENTO { get; set; }

        [StringLength(6)]
        public string TP_RECEPCION { get; set; }

        [StringLength(50)]
        public string DS_TIPO_RECEPCION { get; set; }
    }
}
