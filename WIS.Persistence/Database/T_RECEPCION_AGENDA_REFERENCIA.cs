namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_RECEPCION_AGENDA_REFERENCIA")]
    public partial class T_RECEPCION_AGENDA_REFERENCIA
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [Key]
        [StringLength(40)]
        [Column(Order = 1)]
        public string CD_PRODUTO { get; set; }

        [Key]

        [StringLength(40)]
        [Column(Order = 2)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_RECEPCION_REFERENCIA_DET { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public decimal? QT_AGENDADA { get; set; }

        public long? NU_INTERFAZ_EJECUCION { get; set; }

        public virtual T_RECEPCION_REFERENCIA_DET T_RECEPCION_REFERENCIA_DET { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }
    }
}
