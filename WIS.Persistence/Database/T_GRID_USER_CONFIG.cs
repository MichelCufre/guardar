namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_GRID_USER_CONFIG")]
    public partial class T_GRID_USER_CONFIG
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(30)]
        public string CD_APLICACION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string CD_BLOQUE { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(30)]
        public override string NM_DATAFIELD { get; set; }

        [StringLength(100)]
        public override string DS_COLUMNA { get; set; }

        public override short? NU_ORDEN_VISUAL { get; set; }

        [Required]
        [StringLength(1)]
        public override string FL_VISIBLE { get; set; }

        public int? RESOURCEID { get; set; }

        [Required]
        [StringLength(1)]
        public override string VL_ALINEACION { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_DATA_FORMAT_STRING { get; set; }

        public override decimal? VL_WIDTH { get; set; }

        [Required]
        [StringLength(2)]
        public override string VL_TYPE { get; set; }

        [StringLength(500)]
        [Column]
        public string VL_LINK { get; set; }

        public override short? VL_POSICION_FIJADO { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int USERID { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_TRADUCIR { get; set; }
    }
}
