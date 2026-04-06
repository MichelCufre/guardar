using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("T_CODIGO_MULTIDATO_EMPRESA_DET")]
    public partial class T_CODIGO_MULTIDATO_EMPRESA_DET
    {

        [Key]
        [Column(Order = 0)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(30)]
        public string CD_CODIGO_MULTIDATO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(30)]
        public string CD_APLICACION { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(30)]
        public string CD_CAMPO { get; set; }

        [Key]
        [StringLength(30)]
        public string CD_AI { get; set; }

        public short NU_ORDEN { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }


    }
}
