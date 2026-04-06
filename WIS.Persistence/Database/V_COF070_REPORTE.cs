namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_COF070_REPORTE")]
    public partial class V_COF070_REPORTE
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_REPORTE { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_REPORTE { get; set; }

        public int? CD_USUARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_ARCHIVO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_SITUACION { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_REPORTE_RELACION { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(200)]
        public string CD_CLAVE { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(20)]
        public string NM_TABLA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(100)]
        public string FULLNAME { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO_VALOR { get; set; }

        [StringLength(40)]
        [Column]
        public string DS_REPORTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_RECURSO_TEXTO { get; set; }
    }
}
