namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_POTERIA_VEHICULO_AGENDA")]
    public partial class V_POTERIA_VEHICULO_AGENDA
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PORTERIA_VEHICULO { get; set; }

        [Required]
        [StringLength(15)]
        [Column]
        public string ND_TRANSPORTE { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_DOMINIO_VALOR { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO_VALOR { get; set; }

        [StringLength(10)]
        [Column]
        public string VL_MATRICULA_1 { get; set; }

        [StringLength(10)]
        [Column]
        public string VL_MATRICULA_2 { get; set; }

        public decimal? VL_PESO_ENTRADA { get; set; }

        public decimal? VL_PESO_SALIDA { get; set; }

        public int? CD_EMPRESA { get; set; }

        public DateTime? DT_PORTERIA_ENTRADA { get; set; }

        public DateTime? DT_PORTERIA_SALIDA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PORTERIA_VEHICULO_AGENDA { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_PROVEEDOR { get; set; }

        public int? CD_EMPRESA_AGENDA { get; set; }

        [StringLength(3)]
        [Column]
        public string CD_TIPO_DOCUMENTO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO { get; set; }

        public short? CD_SITUACAO { get; set; }

        public short? CD_OPERACAO { get; set; }

        public DateTime? DT_ADDROW_AGENDA { get; set; }

        public DateTime? DT_UPDROW_AGENDA { get; set; }

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
    }
}
