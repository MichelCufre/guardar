using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STO498_PALLET_TRANSFERENCIA")]
    public partial class V_STO498_PALLET_TRANSFERENCIA
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

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_ENDERECO_ORIGEN { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(30)]
        public string DS_SITUACAO { get; set; }

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

        [Required]
        [StringLength(30)]
        [Column]
        public string NM_FUNCIONARIO { get; set; }

        [Required]
        [StringLength(50)]
        [Column]
        public string LOGINNAME { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_NAM { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string CD_MERCADOLOGICO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string CD_UNIDADE_MEDIDA { get; set; }

        public int CD_FAMILIA_PRODUTO { get; set; }

        [Required]
        [StringLength(2)]
        [Column]
        public string CD_CLASSE { get; set; }

        [StringLength(20)]
        [Column]
        public string DS_REDUZIDA { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_SEC_DETALLE { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
        [StringLength(50)]
        [Column]
        public string ID_EXTERNO_ETIQUETA { get; set; }
        [StringLength(20)]
        [Column]
        public string TP_ETIQUETA_TRANSFERENCIA { get; set; }

        public long? NU_LPN { get; set; }

		[StringLength(20)]
		[Column]
		public string TP_MODALIDAD_USO { get; set; }

        [StringLength(200)]
        public string VL_METADATA { get; set; }
    }
}
