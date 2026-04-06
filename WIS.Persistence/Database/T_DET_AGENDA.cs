namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DET_AGENDA")]
    public partial class T_DET_AGENDA
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        public short? CD_SITUACAO { get; set; }

        public decimal? QT_AGENDADO { get; set; }

        public decimal? QT_CROSS_DOCKING { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        public decimal? VL_PRECIO { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public DateTime? DT_ACEPTADA_RECEPCION { get; set; }

        public int? CD_FUNC_ACEPTO_RECEPCION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? QT_ACEPTADA { get; set; }

        public decimal? QT_AGENDADO_ORIGINAL { get; set; }

        public decimal? QT_RECIBIDA_FICTICIA { get; set; }

        public decimal? VL_CIF { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public virtual T_AGENDA T_AGENDA { get; set; }
    }
}
