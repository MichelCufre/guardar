using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_PRE110_ATRIBUTOS_DEFINIDOS")]
    public partial class V_PRE110_ATRIBUTOS_DEFINIDOS
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
        public long NU_DET_PED_SAI_ATRIB { get; set; }

        [Key]
        [Column(Order = 8)]
        public int ID_ATRIBUTO { get; set; }

        [Required]
        [StringLength(50)]
        public string NM_ATRIBUTO { get; set; }

        [StringLength(400)]
        public string DS_ATRIBUTO { get; set; }

        [Required]
        [StringLength(10)]
        public string ID_ATRIBUTO_TIPO { get; set; }

        [Required]
        [StringLength(100)]
        public string DS_ATRIBUTO_TIPO { get; set; }

        [Required]
        [StringLength(400)]
        public string VL_ATRIBUTO { get; set; }

        [Key]
        [Column(Order = 14)]
        [StringLength(1)]
        public string FL_CABEZAL { get; set; }

    }
}
