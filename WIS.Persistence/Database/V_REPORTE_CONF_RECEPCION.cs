namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REPORTE_CONF_RECEPCION")]
    public partial class V_REPORTE_CONF_RECEPCION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(3)]
        [Column]
        public string CD_TIPO_DOCUMENTO { get; set; }

        [StringLength(20)]
        [Column]
        public string NU_DOCUMENTO { get; set; }

        public short? CD_SITUACAO { get; set; }

        public short? CD_OPERACAO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public short? CD_PORTA { get; set; }

        public DateTime? DT_INICIO { get; set; }

        public DateTime? DT_FIN { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_PLACA { get; set; }

        [StringLength(25)]
        [Column]
        public string NU_DUA { get; set; }

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
        public string ID_ENVIO_DOCUMENTACION { get; set; }

        public int? CD_FUNC_ENVIO_DOCU { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AVERIA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_FECHA_VENCIMIENTO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PESO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_VOLUMEN { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        public DateTime? DT_FACTURACION { get; set; }

        public DateTime? DT_CIERRE { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_RECEPCION { get; set; }

        public int? CD_FUNCIONARIO_ASIGNADO { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_RECEPCION_EXTERNO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

        public short CD_ROTA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ENDERECO { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_BAIRRO { get; set; }

        public long? CD_GLN { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_INSCRICAO { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_TELEFONE { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_CEP { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_FAX { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_CGC_CLIENTE { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO_CLIENTE1 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO_CLIENTE2 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO_CLIENTE3 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO_CLIENTE4 { get; set; }

        [Required]
        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ROTA { get; set; }

        public long? ID_LOCALIDAD { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_CIUDAD { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_LUGAR { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_SUBDIV { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_SUBDIVISION { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_PAIS { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_PAIS { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_LUGAR { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_RECEPCION_EXTERNO { get; set; }
    }
}
