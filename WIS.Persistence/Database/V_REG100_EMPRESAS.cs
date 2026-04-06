namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REG100_EMPRESAS")]
    public partial class V_REG100_EMPRESAS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_CGC_EMPRESA { get; set; }

        public short CD_SITUACAO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ENDERECO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_NOTIFICACION { get; set; }

        [StringLength(15)]
        [Column]
        public string DS_CP_POSTAL { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_TELEFONE { get; set; }

        public decimal? IM_MINIMO_STOCK { get; set; }

        public short? TP_ALMACENAJE_Y_SEGURO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO4 { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE_ARMADO_KIT { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_PAIS { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_PAIS { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_SUBDIVISION { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_SUBDIVISION { get; set; }

        public long? ID_LOCALIDAD { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_LOCALIDAD { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_LOCALIDAD { get; set; }

        [StringLength(1)]
        [Column]
        public string ACTIVO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_TIPO_FISCAL { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_TIPO_FISCAL { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_FISCAL { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ALMACENAJE_Y_SEGURO { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_NOTIFICACION { get; set; }
    }
}
