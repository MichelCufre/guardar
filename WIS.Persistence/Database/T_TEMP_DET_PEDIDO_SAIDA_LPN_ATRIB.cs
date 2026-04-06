using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB")]
    public partial class T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(1)]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }

        [Key]
        [Column(Order = 9)]
        public int ID_ATRIBUTO { get; set; }

        [Key]
        [Column(Order = 10)]
        public int USERID { get; set; }

        [Key]
        [Column(Order = 11)]
        [StringLength(1)]
        public string FL_CABEZAL { get; set; }

        [StringLength(400)]
        public string VL_ATRIBUTO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }
    }
}
