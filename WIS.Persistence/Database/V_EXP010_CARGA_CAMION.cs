using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_EXP010_CARGA_CAMION
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }
        [Key]
        [Column(Order = 1)]
        public int CD_EMPRESA { get; set; }
        [Key]
        [Column(Order = 2)]
        public int NU_PREPARACION { get; set; }
        [Key]
        [Column(Order = 3)]
        public long NU_CARGA { get; set; }
        public short? CD_ROTA_CARGA { get; set; }
        [StringLength(30)]
        [Column]
        public string DS_ROTA_CARGA { get; set; }
        public short? CD_SITUACAO_CAMION { get; set; }
        public int? CD_CAMION { get; set; }
        [StringLength(1)]
        [Column]
        public string ID_CARGAR { get; set; }
        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
        [StringLength(6)]
        [Column]
        public string CD_GRUPO_EXPEDICION { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_PEDIDO { get; set; }

        [Column]
        [StringLength(1)]
        public string TP_EXP_MANEJA_TRACKING { get; set; }

        [Column]
        [StringLength(200)]
        public string VL_COMPARTE_CONTENEDOR_ENTREGA { get; set; }

	}
}
