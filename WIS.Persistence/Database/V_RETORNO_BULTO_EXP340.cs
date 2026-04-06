namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_RETORNO_BULTO_EXP340")]
    public partial class V_RETORNO_BULTO_EXP340
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_RETORNO_BULTO { get; set; }

        public int? CD_CAMION_RETORNO { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_CAMION { get; set; }

        [Required]
        [StringLength(5)]
        [Column]
        public string TP_RETORNO { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_TIPO_RETORNO { get; set; }

        public int NU_PREPARACION { get; set; }

        public int NU_CONTENEDOR { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_CONFIRMADO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_MOTIVO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_MOTIVO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public short? CD_SITUACAO_CONTENEDOR { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

		[StringLength(50)]
		[Column]
		public string ID_EXTERNO_CONTENEDOR { get; set; }
	}
}
