using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_CONT_PRODUTO_WSTO150DET")]
    public partial class V_CONT_PRODUTO_WSTO150DET
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_CONTENEDOR { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        public long? NU_CARGA { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public int CD_EMPRESA { get; set; }

        [Key]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [StringLength(10)]
        public string MAX_CLIENTE { get; set; }

        [StringLength(10)]
        public string MIN_CLIENTE { get; set; }

		[StringLength(50)]
		public string ID_EXTERNO_CONTENEDOR { get; set; }
	}
}
