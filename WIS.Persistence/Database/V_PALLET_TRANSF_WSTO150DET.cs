namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PALLET_TRANSF_WSTO150DET")]
    public partial class V_PALLET_TRANSF_WSTO150DET
    {
        [Key]
        [Column(Order = 0)]
        public decimal NU_ETIQUETA { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_SEC_ETIQUETA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_REAL { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_ORIGEN { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public short? CD_SITUACAO { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AVERIA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_INVENTARIO_CICLICO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_DESTINO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_ASIGNADO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }
    }
}
