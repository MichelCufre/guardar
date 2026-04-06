using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_EXP011_CONTENEDOR_CAMION
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
        [Column(Order = 4)]
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
        [Key]
        [Column(Order = 3)]
        public int NU_CONTENEDOR { get; set; }
        public short? CD_SITUACAO_CONTENEDOR { get; set; }
        [StringLength(40)]
        [Column]
        public string NU_PEDIDO { get; set; }
        [StringLength(100)]
        [Column]
        public string DS_ENDERECO_PEDIDO { get; set; }
        public int? CD_CAMION_FACTURADO { get; set; }
        [StringLength(20)]
        [Column]
        public string ID_PRECINTO_1 { get; set; }
        [StringLength(20)]
        [Column]
        public string ID_PRECINTO_2 { get; set; }
        [Column]
        public string NU_PREDIO { get; set; }
        [StringLength(1)]
        [Column]
        public string ID_AGRUPACION { get; set; }
        [Column]
        public string CD_GRUPO_EXPEDICION { get; set; }
        [StringLength(60)]
        [Column]
        public string DS_CONTENEDOR { get; set; }
        [StringLength(1)]
        [Column]
        public string VL_CONTROL { get; set; }

        [Column]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [Column]
        [StringLength(1)]
        public string TP_EXP_MANEJA_TRACKING { get; set; }

        [Column]
        [StringLength(200)]
        public string VL_COMPARTE_CONTENEDOR_ENTREGA { get; set; }

		[StringLength(50)]
		[Column]
		public string ID_EXTERNO_CONTENEDOR { get; set; }


        [StringLength(40)]
        public string NU_EXTERNO_UNIDAD { get; set; }

        [StringLength(3)]
        public string TP_UNIDAD_TRANSPORTE { get; set; }

        [StringLength(50)]
        public string CD_BARRAS_UT { get; set; }

        [StringLength(10)]
        public string DT_ADDROW_PEDIDO { get; set; }

        [StringLength(10)]
        public string DT_EMITIDO_PEDIDO { get; set; }

        [StringLength(10)]
        public string DT_ENTREGA_PEDIDO { get; set; }
    }
}
