using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_CODIGO_MULTIDATO_EMPRESA_DET")]
    public partial class V_CODIGO_MULTIDATO_EMPRESA_DET
    {

        [Key]
        [Column(Order = 0)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(30)]
        public string CD_CODIGO_MULTIDATO { get; set; }

        [StringLength(100)]
        public string DS_CODIGO_MULTIDATO { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(30)]
        public string CD_APLICACION { get; set; }

        [StringLength(100)]
        public string DS_APLICACION { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(30)]
        public string CD_CAMPO { get; set; }

        [StringLength(100)]
        public string DS_CAMPO { get; set; }

        [Key]
        [StringLength(30)]
        public string CD_AI { get; set; }

        [StringLength(100)]
        public string DS_AI { get; set; }

        public short NU_ORDEN { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public short QT_AIS { get; set; }
    }
}
