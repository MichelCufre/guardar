namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PORTERIA_AGENDAS")]
    public partial class V_PORTERIA_AGENDAS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

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

        [StringLength(30)]
        [Column]
        public string DS_PLACA { get; set; }

        [StringLength(3)]
        [Column]
        public string CD_TIPO_DOCUMENTO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO { get; set; }

        [StringLength(25)]
        [Column]
        public string NU_DUA { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_RECEPCION { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_RECEPCION_EXTERNO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }
    }
}
