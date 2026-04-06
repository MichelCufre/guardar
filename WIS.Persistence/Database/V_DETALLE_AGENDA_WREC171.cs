namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DETALLE_AGENDA_WREC171")]
    public partial class V_DETALLE_AGENDA_WREC171
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        public decimal? QT_AGENDADO_ORIGINAL { get; set; }

        public decimal? QT_AGENDADO { get; set; }

        public decimal? PESO_AGENDADO { get; set; }

        public decimal? VOLUMEN_AGENDADO { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public decimal? PESO_RECIBIDO { get; set; }

        public decimal? VOLUMEN_RECIBIDO { get; set; }

        public long? CANT_ETIQUETAS { get; set; }

        public DateTime? DT_ACEPTADA_RECEPCION { get; set; }

        public int? CD_FUNC_ACEPTO_RECEPCION { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_FUNCIONARIO { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_CLASSE { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_FAMILIA_PRODUTO { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_UNIDADE_MEDIDA { get; set; }

        public decimal? QT_UND_BULTO { get; set; }

        public decimal? QT_CONV { get; set; }

        public decimal? QT_CONV_AGENDADA { get; set; }

        public DateTime? DT_ENTRADA { get; set; }

        public decimal? VL_PRECO_VENDA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
