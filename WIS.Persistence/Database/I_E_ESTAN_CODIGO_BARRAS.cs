namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("I_E_ESTAN_CODIGO_BARRAS")]
    public partial class I_E_ESTAN_CODIGO_BARRAS
    {
        [StringLength(1)]
        [Column]
        public string ID_PROCESADO { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_REGISTRO { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_BARRAS { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [StringLength(8)]
        [Column]
        public string TP_CODIGO_BARRAS { get; set; }

        [StringLength(10)]
        [Column]
        public string DT_UPDROW { get; set; }

        [StringLength(10)]
        [Column]
        public string DT_ADDROW { get; set; }

        [StringLength(30)]
        [Column]
        public string QT_EMBALAGEM { get; set; }

        [StringLength(2)]
        [Column]
        public string NU_PRIORIDADE_USO { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_CARGA { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_OPERACION { get; set; }

        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_REGISTRO_PADRE { get; set; }

        [Column]
        public DateTime? DT_ADDROW_INTERFAZ { get; set; }
    }
}
