namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_ETIQUETA_PRE_SEP_WREC270")]
    public partial class V_ETIQUETA_PRE_SEP_WREC270
    {
        public int NU_AGENDA { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_ETIQUETA_LOTE { get; set; }

        [Required]
        [StringLength(40)]
        public string NU_EXTERNO_ETIQUETA { get; set; }

        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public short? CD_SITUACAO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(100)]
        public string DS_CLIENTE { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        public short? MIN_SITUACAO { get; set; }

        public short? MAX_SITUACAO { get; set; }

        [StringLength(1)]
        public string ID_CTRL_ACEPTADO { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }
    }
}
