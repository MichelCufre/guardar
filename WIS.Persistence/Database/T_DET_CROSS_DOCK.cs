namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DET_CROSS_DOCK")]
    public partial class T_DET_CROSS_DOCK
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }
        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }
        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }
        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }
        [Key]
        [Column(Order = 6)]
        public decimal CD_FAIXA { get; set; }
        [Key]
        [Column(Order = 7)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }
        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        public long NU_CARGA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        public decimal? QT_PRODUTO { get; set; }
        [Key]
        [Column(Order = 9)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION_PICKEO { get; set; }
        public decimal? QT_PREPARADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public long? NU_TRANSACCION { get; set; }

        public virtual T_CROSS_DOCK T_CROSS_DOCK { get; set; }
    }
}
