namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("I_S_ESTAN_PEDIDO_ANULADO")]
    public partial class I_S_ESTAN_PEDIDO_ANULADO
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
        [StringLength(20)]
        public string NU_REGISTRO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [StringLength(9)]
        [Column]
        public string CD_FAIXA { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_PEDIDO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string QT_ANULADO { get; set; }

        [StringLength(7)]
        [Column]
        public string CD_FUNCIONARIO { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_MOTIVO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_APLICACAO { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_ADDROW { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        public DateTime? DT_ADDROW_INTERFAZ { get; set; }
    }
}
