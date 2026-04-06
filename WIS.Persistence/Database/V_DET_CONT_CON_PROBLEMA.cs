namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DET_CONT_CON_PROBLEMA")]
    public partial class V_DET_CONT_CON_PROBLEMA
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_SEQ_PREPARACION { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        public int? NU_CONTENEDOR { get; set; }

        public long? NU_CARGA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 7)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_PICKEO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int? NU_CONTENEDOR_PICKEO { get; set; }

        public int? CD_FUNC_PICKEO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AGRUPACION { get; set; }

        public int? NU_CONTENEDOR_SYS { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ERROR_CONTROL { get; set; }

        public int? CD_CAMION_1 { get; set; }
        public int? CD_CAMION_2 { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(1)]
        [Column]
        public string COMPARTIDO { get; set; }

        [StringLength(2)]
        [Column]
        public string NU_PREDIO { get; set; }

		[StringLength(50)]
		[Column]
		public string ID_EXTERNO_CONTENEDOR { get; set; }
	}
}
