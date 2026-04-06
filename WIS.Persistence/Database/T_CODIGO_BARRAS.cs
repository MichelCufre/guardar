namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_CODIGO_BARRAS")]
    public partial class T_CODIGO_BARRAS
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string CD_BARRAS { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        public int? TP_CODIGO_BARRAS { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public decimal? QT_EMBALAGEM { get; set; }

        public short? NU_PRIORIDADE_USO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public virtual T_TIPO_CODIGO_BARRAS T_TIPO_CODIGO_BARRAS { get; set; }
    }
}
