namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_EVENTO_AGENDA_CERRADAS")]
    public partial class V_EVENTO_AGENDA_CERRADAS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_PROVEEDOR { get; set; }
        [StringLength(40)]
        [Column]
        public string CD_EMPRESA { get; set; }

        [StringLength(3)]
        [Column]
        public string CD_TIPO_DOCUMENTO { get; set; }

        [StringLength(10)]
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

        public long? NU_INTERFAZ_EJECUCION { get; set; }

        public int? NU_ORT_ORDEN { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public decimal? VL_TEMP_CAMION { get; set; }

        public decimal? VL_TEMP_INICIO_REC { get; set; }

        public decimal? VL_TEMP_MITAD_REC { get; set; }

        public decimal? VL_TEMP_FINAL_REC { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }
    }
}
