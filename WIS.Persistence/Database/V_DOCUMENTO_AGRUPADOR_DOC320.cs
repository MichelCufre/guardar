using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_DOCUMENTO_AGRUPADOR_DOC320")]
    public partial class V_DOCUMENTO_AGRUPADOR_DOC320
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

        public decimal? QT_PESO_LIQUIDO { get; set; }

        public DateTime? DT_SAIDA { get; set; }

        [StringLength(20)]
        public string DS_PLACA { get; set; }

        [StringLength(100)]
        public string DS_MOTORISTA { get; set; }

        [StringLength(64)]
        public string DS_TIPO { get; set; }

        public int? CD_TRANSPORTADORA { get; set; }

        [StringLength(100)]
        public string DS_TRANSPORTADORA { get; set; }

        public int? CD_TIPO_VEICULO { get; set; }

        [StringLength(20)]
        public string DS_TIPO_VEICULO { get; set; }

        [StringLength(200)]
        public string ANEXO1 { get; set; }

        [StringLength(200)]
        public string ANEXO2 { get; set; }

        [StringLength(200)]
        public string ANEXO3 { get; set; }

        [StringLength(200)]
        public string ANEXO4 { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }
    }
}
