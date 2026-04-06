namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DET_PICKING_CAMION_WEXP040")]
    public partial class V_DET_PICKING_CAMION_WEXP040
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_CAMION { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_CARGA { get; set; }

        public int NU_PREPARACION { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        public int? NU_CONTENEDOR { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_EMPAQUETA_CONTENEDOR { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PERMITE_FACT_SIN_PRECINTO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AGRUPACION { get; set; }
    }
}
