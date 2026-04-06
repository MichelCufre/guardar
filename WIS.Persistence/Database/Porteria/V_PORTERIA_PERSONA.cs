namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PORTERIA_PERSONA")]
    public partial class V_PORTERIA_PERSONA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PORTERIA_REGISTRO_PERSONA { get; set; }

        [Required]
        [StringLength(15)]
        [Column]
        public string ND_TP_POTERIA_REGISTRO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_TP_POTERIA_REGISTRO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TP_POTERIA_REGISTRO { get; set; }

        [StringLength(15)]
        [Column]
        public string ND_POTERIA_MOTIVO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_POTERIA_MOTIVO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_POTERIA_MOTIVO { get; set; }

        public int CD_FUNCIONARIO { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_FUNCIONARIO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

        public int? NU_POTERIA_PERSONA { get; set; }

        [StringLength(20)]
        [Column]
        public string NU_DOCUMENTO { get; set; }

        [StringLength(15)]
        [Column]
        public string ND_TP_DOCUMENTO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_TP_DOCUMENTO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TP_DOCUMENTO { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_PAIS_EMISOR { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_PERSONA { get; set; }

        [StringLength(100)]
        [Column]
        public string AP_PERSONA { get; set; }

        [StringLength(15)]
        [Column]
        public string NU_CELULAR { get; set; }

        [StringLength(15)]
        [Column]
        public string ND_TP_PERSONA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_TP_PERSON { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TP_PERSON { get; set; }

        [StringLength(15)]
        [Column]
        public string ND_PUESTO_FUNCIONARIO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_PUESTO_FUNCIONARIO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_PUESTO_FUNCIONARIO { get; set; }

        public DateTime? DT_PERSONA_ENTRADA { get; set; }

        public DateTime? DT_PERSONA_SALIDA { get; set; }

        public int? CD_EMPRESA_PERSONA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA_PERSONA { get; set; }

        public int? NU_PORTERIA_VEHICULO_ENTRADA { get; set; }

        [StringLength(15)]
        [Column]
        public string ND_TRANSPORTE_ENTRADA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_TRANSPORTE_V1 { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TRANSPORTE_V1 { get; set; }

        [StringLength(10)]
        [Column]
        public string VL_MATRICULA_1_ENTRADA { get; set; }

        [StringLength(10)]
        [Column]
        public string VL_MATRICULA_2_ENTRADA { get; set; }

        public decimal? VL_PESO_ENTRADA_VE { get; set; }

        public decimal? VL_PESO_SALIDA_VE { get; set; }

        public int? CD_EMPRESA_ENTRADA { get; set; }

        public DateTime? DT_PORTERIA_ENTRADA_VE { get; set; }

        public DateTime? DT_PORTERIA_SALIDA_VE { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SALIDA_HABILITADA_V1 { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CONTROL_SALIDA_V1 { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SOLO_BALANZA_V1 { get; set; }

        public int? NU_PORTERIA_VEHICULO_SALIDA { get; set; }

        [StringLength(15)]
        [Column]
        public string ND_TRANSPORTE_SALIDA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_TRANSPORTE_V2 { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TRANSPORTE_V2 { get; set; }

        [StringLength(10)]
        [Column]
        public string VL_MATRICULA_1_SALIDA { get; set; }

        [StringLength(10)]
        [Column]
        public string VL_MATRICULA_2_SALIDA { get; set; }

        public decimal? VL_PESO_ENTRADA_VS { get; set; }

        public decimal? VL_PESO_SALIDA_VS { get; set; }

        public int? CD_EMPRESA_SALIDA { get; set; }

        public DateTime? DT_PORTERIA_ENTRADA_VS { get; set; }

        public DateTime? DT_PORTERIA_SALIDA_VS { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SALIDA_HABILITADA_V2 { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CONTROL_SALIDA_V2 { get; set; }

        [Required]
        [StringLength(15)]
        [Column]
        public string ND_SECTOR { get; set; }

        [StringLength(400)]
        [Column]
        public string DS_NOTA { get; set; }

        [Required]
        [StringLength(15)]
        [Column]
        public string ND_ESTADO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_ESTADO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ESTADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_SECTOR { get; set; }

        [StringLength(300)]
        [Column]
        public string DS_SECTOR { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
