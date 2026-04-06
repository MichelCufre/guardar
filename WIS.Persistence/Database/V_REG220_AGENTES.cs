namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REG220_AGENTES")]
    public partial class V_REG220_AGENTES
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ENDERECO { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_BAIRRO { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_CEP { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_TELEFONE { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_FAX { get; set; }

        public long? CD_GLN { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_CGC_CLIENTE { get; set; }

        public short CD_ROTA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ROTA { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_INSCRICAO { get; set; }

        public short? NU_PRIOR_CARGA { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        public DateTime? DT_SITUACAO { get; set; }

        public DateTime? DT_CADASTRAMENTO { get; set; }

        public DateTime? DT_ALTERACAO { get; set; }

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

        [StringLength(1)]
        [Column]
        public string ACTIVO { get; set; }

        public long? ID_LOCALIDAD { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_LOCALIDAD { get; set; }

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

        [StringLength(20)]
        [Column]
        public string ND_TIPO_FISCAL { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_DOMINIO_VALOR { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_FISCAL { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_AGENTE { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SYNC_REALIZADA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_EMAIL { get; set; }
    }
}
