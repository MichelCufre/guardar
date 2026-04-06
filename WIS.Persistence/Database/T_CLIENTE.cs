namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_CLIENTE")]
    public partial class T_CLIENTE
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        public short CD_ROTA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

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

        [StringLength(15)]
        [Column]
        public string NU_DDD { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_FAX { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_INSCRICAO { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_CGC_CLIENTE { get; set; }

        public short? CD_SITUACAO { get; set; }

        public DateTime? DT_SITUACAO { get; set; }

        public DateTime? DT_CADASTRAMENTO { get; set; }

        public DateTime? DT_ALTERACAO { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_ATIVIDADE { get; set; }

        public short? NU_PRIOR_CARGA { get; set; }

        public short? NU_DV_CLIENTE { get; set; }

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
        public string ID_CLIENTE_FILIAL { get; set; }

        public int? CD_FORNECEDOR { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE_EN_CONSOLIDADO { get; set; }

        public int? CD_EMPRESA_CONSOLIDADA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ACEPTA_DEVOLUCION { get; set; }

        public long? CD_GLN { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_PUNTO_ENTREGA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CATEGORIA { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_TIPO_FISCAL { get; set; }

        public long? ID_LOCALIDAD { get; set; }

        public decimal? VL_PORCENTAJE_VIDA_UTIL { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_GRUPO_CONSULTA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SYNC_REALIZADA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_EMAIL { get; set; }

        public virtual T_ROTA T_ROTA { get; set; }
        public virtual ICollection<T_CLIENTE_RUTA_PREDIO> T_CLIENTE_RUTA_PREDIO { get; set; }

        public virtual T_PAIS_SUBDIVISION_LOCALIDAD T_PAIS_SUBDIVISION_LOCALIDAD { get; set; }
    }
}
