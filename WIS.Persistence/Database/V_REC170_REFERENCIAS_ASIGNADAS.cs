using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_REC170_REFERENCIAS_ASIGNADAS")]
    public class V_REC170_REFERENCIAS_ASIGNADAS
    {

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_RECEPCION_REFERENCIA { get; set; }
        public int NU_AGENDA_REFERENCIA_REL { get; set; }

        [StringLength(20)]
        [Column]
        public string NU_REFERENCIA { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_REFERENCIA { get; set; }

        public int CD_EMPRESA { get; set; }

        public DateTime? DT_ENTREGA { get; set; }

        public DateTime? DT_VENCIMIENTO_ORDEN { get; set; }

        public DateTime? DT_EMITIDA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_MEMO { get; set; }

        [StringLength(200)]
        [Column]
        public string VL_SERIALIZADO { get; set; }

        public int CD_SITUACAO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_INTERFAZ_EJECUCION { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO3 { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_AGENTE { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ESTADO_REFERENCIA { get; set; }


    }
}
