using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STO710_CONSULTA_LPN_ATRIBUTOS")]
    public partial class V_STO710_CONSULTA_LPN_ATRIBUTOS
    {
        [Key]
        public int CD_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [Key]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [StringLength(400)]
        [Column]
        public string DS_ATRIBUTO { get; set; }

        [StringLength(400)]
        [Column]
        public string DS_LPN_TIPO { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [Key]
        public int ID_ATRIBUTO { get; set; }

        [StringLength(50)]
        public string NM_ATRIBUTO { get; set; }

        [Key]
        public int ID_LPN_DET { get; set; }

        [StringLength(40)]
        [Column]
        public string ID_LINEA_SISTEMA_EXTERNO { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        public long NU_LPN { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_LPN_EXTERNO { get; set; }

        [Key]
        [StringLength(10)]
        [Column]
        public string TP_LPN_TIPO { get; set; }

        [StringLength(400)]
        [Column]
        public string VL_LPN_ATRIBUTO { get; set; }        
        
        [StringLength(1)]
        public string TP_ATRIBUTO_ASOCIADO { get; set; }

        [StringLength(6)]
        public string ID_ESTADO_LPN { get; set; }

        [StringLength(100)]
        public string DS_ESTADO_LPN { get; set; }

        [StringLength(6)]
        public string ID_ESTADO_ATRIBUTO { get; set; }

        [StringLength(100)]
        public string DS_ESTADO_ATRIBUTO { get; set; }
    }
}