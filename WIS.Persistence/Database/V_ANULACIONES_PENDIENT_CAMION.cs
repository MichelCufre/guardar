using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_ANULACIONES_PENDIENT_CAMION
    {
        [Key]
        [Column(Order = 0)]
        public int CD_CAMION { get; set; }
        [Key]
        [Column(Order = 1)]
        public int CD_EMPRESA { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CD_CLIENTE { get; set; }
        [Key]
        [Column(Order = 3)]
        public long NU_CARGA { get; set; }
        [Key]
        [Column(Order = 4)]
        public string NU_PEDIDO { get; set; }
        [Key]
        [Column(Order = 5)]
        public int NU_CONTENEDOR { get; set; }
        [Key]
        [Column(Order = 6)]
        public string CD_PRODUTO { get; set; }
        [Key]
        [Column(Order = 7)]
        public int NU_PREPARACION { get; set; }
    }
}
