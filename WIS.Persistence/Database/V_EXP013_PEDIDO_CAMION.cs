using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_EXP013_PEDIDO_CAMION
    {
		[Key]
		public int CD_EMPRESA { get; set; }
		
		[Key]
		[StringLength(10)]
		[Column]
        public string CD_CLIENTE { get; set; }

		[Key]
		[StringLength(40)]
		[Column]
        public string NU_PEDIDO { get; set; }
		
		[StringLength(100)]
		[Column]
        public string DS_ENDERECO { get; set; }
		
		public DateTime? DT_ADDROW { get; set; }
		
		public DateTime? DT_ENTREGA { get; set; }
		
		[StringLength(40)]
		[Column]
        public string CD_AGENTE { get; set; }
		
		[StringLength(3)]
		[Column]
        public string TP_AGENTE { get; set; }

		[StringLength(100)]
		[Column]
        public string DS_CLIENTE { get; set; }

		public int? CD_CAMION { get; set; }

		[StringLength(1)]
		[Column]
        public string ID_CARGAR { get; set; }

		public int? CD_ROTA { get; set; }

		[StringLength(30)]
		[Column]
        public string DS_ROTA { get; set; }

		[StringLength(10)]
		[Column]
        public string NU_PREDIO { get; set; }

		[StringLength(6)]
		[Column]
        public string CD_GRUPO_EXPEDICION { get; set; }

		[StringLength(6)]
		[Column]
		public string TP_PEDIDO { get; set; }
		
		[StringLength(60)]
		[Column]
		public string DS_TIPO_PEDIDO { get; set; }
		
		[StringLength(6)]
		[Column]
		public string TP_EXPEDICION { get; set; }
		
		[StringLength(60)]
		[Column]
		public string NM_EXPEDICION { get; set; }

		[Column]
		[StringLength(1)]
		public string TP_EXP_MANEJA_TRACKING { get; set; }

		[Column]
		[StringLength(200)]
		public string VL_COMPARTE_CONTENEDOR_ENTREGA { get; set; }

		public short? CD_SITUACAO_CAMION { get; set; }

		[Column]
		[StringLength(1)]
		public string TIENE_PENDIENTES { get; set; }

        public DateTime? DT_EMITIDO{ get; set; }

        [StringLength(200)]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO4 { get; set; }
    }
}
