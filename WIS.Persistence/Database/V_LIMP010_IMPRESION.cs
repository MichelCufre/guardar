namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_LIMP010_IMPRESION")]
    public partial class V_LIMP010_IMPRESION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_IMPRESION { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_IMPRESORA { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(100)]
        [Column]
        public string FULLNAME { get; set; }

        public DateTime? DT_GENERADO { get; set; }

        public DateTime? DT_PROCESADO { get; set; }

        [StringLength(150)]
        [Column]
        public string DS_REFERENCIA { get; set; }

        public int? QT_REGISTROS { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ESTADO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_DOMINIO_VALOR { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO_VALOR { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ERROR { get; set; }
    }
}
