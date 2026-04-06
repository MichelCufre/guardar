using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_REG070_ZONA_UBICACION")]
    public class V_REG070_ZONA_UBICACION
    {

        [Key]
        [StringLength(20)]
        [Column]
        public string CD_ZONA_UBICACION { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ZONA_UBICACION { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_ZONA_UBICACION_PICKING { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_ZONA_UBICACION { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_ESTACION { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_ESTACION_ALMACENAJE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_ZONA_UBICACION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_HABILITADA { get; set; }
        
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        [Column]
        public int ID_ZONA_UBICACION { get; set; }
    }
}
