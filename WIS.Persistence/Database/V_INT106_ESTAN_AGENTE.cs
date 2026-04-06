namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_INT106_ESTAN_AGENTE")]
    public partial class V_INT106_ESTAN_AGENTE
    {
        [StringLength(100)]
        [Column]
        public string DS_TIPO_AGENTE { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_AGENTE { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_CEP { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_CGC_CLIENTE { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(16)]
        [Column]
        public string CD_GLN { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_LUGAR { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_ROTA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ROTA { get; set; }

        [StringLength(3)]
        [Column]
        public string CD_SITUACAO { get; set; }

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

        [StringLength(50)]
        [Column]
        public string DS_BAIRRO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ENDERECO { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_ADDROW { get; set; }

        public DateTime? DT_ADDROW_INTERFAZ { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PROCESADO { get; set; }

        [StringLength(15)]
        [Column]
        public string NU_FAX { get; set; }

        [StringLength(15)]
        [Column]
        public string NU_INSCRICAO { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string NU_REGISTRO { get; set; }

        [StringLength(15)]
        [Column]
        public string NU_TELEFONE { get; set; }
    }
}
