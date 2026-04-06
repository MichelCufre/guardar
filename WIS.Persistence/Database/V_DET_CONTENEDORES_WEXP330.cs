namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DET_CONTENEDORES_WEXP330")]
    public partial class V_DET_CONTENEDORES_WEXP330
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_CONTENEDOR { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }
        
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_TEMPORAL { get; set; }

    }
}
