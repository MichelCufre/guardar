using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_DOCUMENTO_AGRUPADOR")]
    public partial class T_DOCUMENTO_AGRUPADOR
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string NU_AGRUPADOR { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(3)]
        public string TP_AGRUPADOR { get; set; }

        [Required]
        [StringLength(3)]
        public string ID_ESTADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public int? QT_VOLUMEN { get; set; }

        [StringLength(20)]
        public string NU_LACRE { get; set; }

        public decimal? VL_TOTAL { get; set; }

        public decimal? QT_PESO { get; set; }

        public DateTime? DT_SAIDA { get; set; }

        public DateTime? DT_LLEGADA { get; set; }

        [StringLength(64)]
        public string VL_DATO_AUDITORIA { get; set; }

        public DateTime? DT_UPDATEROW { get; set; }

        public int? CD_EMPRESA { get; set; }

        public decimal? QT_PESO_LIQUIDO { get; set; }

        [StringLength(100)]
        public string DS_MOTORISTA { get; set; }

        public int? CD_TRANSPORTADORA { get; set; }

        public int? CD_TIPO_VEICULO { get; set; }

        [StringLength(20)]
        public string DS_PLACA { get; set; }

        [StringLength(200)]
        public string ANEXO1 { get; set; }

        [StringLength(200)]
        public string ANEXO2 { get; set; }

        [StringLength(200)]
        public string ANEXO3 { get; set; }

        [StringLength(200)]
        public string ANEXO4 { get; set; }

        [StringLength(512)]
        public string DS_MOTIVO { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        public DateTime? DT_IMPRESO { get; set; }

        public virtual T_TRANSPORTADORA T_TRANSPORTADORA { get; set; }

        public virtual T_TIPO_VEICULO T_TIPO_VEICULO { get; set; }

        public virtual T_DOCUMENTO_AGRUPADOR_TIPO T_DOCUMENTO_AGRUPADOR_TIPO { get; set; }
    }

}
