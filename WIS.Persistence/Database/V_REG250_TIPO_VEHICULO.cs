using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_REG250_TIPO_VEHICULO
    {
        [Key]
        public int CD_TIPO_VEICULO { get; set; }
        [StringLength(100)]
        [Column]
        public string DS_TIPO_VEICULO { get; set; }
        public decimal? VL_CAPAC_VOLUMEN { get; set; }
        public decimal? VL_CAPAC_PESO { get; set; }
        public decimal? VL_CAPAC_PALLET { get; set; }
        [StringLength(1)]
        [Column]
        public string ID_FRIGORIFICADO { get; set; }
        public short? VL_PORCENTAJE_OCUPACION_VOLU { get; set; }
        public short? VL_PORCENTAJE_OCUPACION_PESO { get; set; }
        public short? VL_PORCENTAJE_OCUPACION_PALLET { get; set; }
        [StringLength(1)]
        [Column]
        public string ID_CARGA_LATERAL { get; set; }
        [StringLength(1)]
        [Column]
        public string ID_ADMITE_ZORRA { get; set; }
        [StringLength(1)]
        [Column]
        public string ID_SOLO_CABINA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SYNC_REALIZADA { get; set; }
    }
}
