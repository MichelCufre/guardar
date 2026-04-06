using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_PRE100_PEDIDO_SAIDA
    {
		[Key]
		[Column(Order = 1)]
		public string CD_CLIENTE { get; set; }
		[Column]
        public string CD_AGENTE { get; set; }
		[Column]
        public string TP_AGENTE { get; set; }
		[Key]
		[Column(Order = 2)]
		public int CD_EMPRESA { get; set; }
		[Column]
        public string CD_ORIGEN { get; set; }
		public int? CD_ROTA { get; set; }
		public short CD_SITUACAO { get; set; }
		[Column]
        public string DS_ANEXO1 { get; set; }
		[Column]
        public string DS_ANEXO2 { get; set; }
		[Column]
        public string DS_ANEXO3 { get; set; }
		[Column]
        public string DS_ANEXO4 { get; set; }
		[Column]
        public string DS_CLIENTE { get; set; }
		[Column]
        public string DS_MEMO { get; set; }
		[Column]
        public string DS_MEMO_1 { get; set; }
		[Column]
        public string DS_ROTA { get; set; }
		[Column]
        public string DS_SITUACAO { get; set; }
		[Column]
		public string DS_TIPO_PEDIDO { get; set; }
		public DateTime? DT_ADDROW { get; set; }
		public DateTime? DT_EMITIDO { get; set; }
		[Column]
        public string DT_ENTREGA { get; set; }
		public DateTime? DT_GENERICO_1 { get; set; }
		public DateTime? DT_LIBERAR_DESDE { get; set; }
		public DateTime? DT_LIBERAR_HASTA { get; set; }
		public DateTime? DT_ULT_PREPARACION { get; set; }
		public DateTime? DT_UPDROW { get; set; }
		[Column]
        public string HR_ENTREGA { get; set; }
		[Column]
        public string ID_AGRUPACION { get; set; }
		[Column]
        public string ID_MANUAL { get; set; }
		[Column]
        public string NM_EMPRESA { get; set; }
		[Column]
        public string NM_EXPEDICION { get; set; }		
		public decimal? NU_GENERICO_1 { get; set; }
		public long? NU_INTERFAZ_FACTURACION { get; set; }
		public short? NU_ORDEN_LIBERACION { get; set; }
		[Key]
		[Column(Order = 0)]
        public string NU_PEDIDO { get; set; }
		[Column]
        public string NU_PRDC_INGRESO { get; set; }
		public int? NU_PREPARACION_MANUAL { get; set; }
		public int? NU_ULT_PREPARACION { get; set; }
		public decimal? QT_ANULADO { get; set; }
		public decimal? QT_EXPEDIDA { get; set; } 
		public decimal? QT_FACTURADO { get; set; }
		public decimal? QT_PEDIDO { get; set; }
		public decimal? QT_LIBERADO { get; set; }
		public decimal? QT_PENDIENTE_LIB { get; set; }
		public decimal? QT_PENDIENTE_PREP { get; set; }
		public decimal? QT_SEPARADO { get; set; }
		
		public decimal? QT_PEND_EXPEDIR { get; set; }
		public decimal? QT_PREPARADO { get; set; }
		[Column]
        public string TP_EXPEDICION { get; set; }
		[Column]
        public string TP_PEDIDO { get; set; }
		[Column]
        public string VL_GENERICO_1 { get; set; }
		[Column]
        public string VL_SERIALIZADO_1 { get; set; }

		public DateTime? DT_FACTURACION { get; set; }

		[StringLength(20)]
		[Column]
		public string CD_PUNTO_ENTREGA { get; set; }

		[StringLength(200)]
		[Column]
		public string VL_COMPARTE_CONTENEDOR_PICKING { get; set; }

		[StringLength(200)]
		[Column]
		public string VL_COMPARTE_CONTENEDOR_ENTREGA { get; set; }

		[StringLength(1)]
		[Column]
		public string FL_SYNC_REALIZADA { get; set; }

		[StringLength(400)]
		[Column]
		public string DS_ENDERECO { get; set; }

		[StringLength(10)]
		[Column]
		public string NU_PREDIO { get; set; }

		public long? NU_CARGA { get; set; }

		public decimal? VL_LATITUD { get; set; }

		public decimal? VL_LONGITUD { get; set; }

		[StringLength(30)]
		[Column]
		public string NU_TELEFONE { get; set; }

		[StringLength(30)]
		[Column]
		public string NU_TELEFONE2 { get; set; }

		public int? CD_TRANSPORTADORA { get; set; }

		[StringLength(100)]
		[Column]
		public string DS_TRANSPORTADORA { get; set; }

		[StringLength(20)]
		[Column]
		public string ND_ACTIVIDAD { get; set; }

        [StringLength(20)]
        public string CD_ZONA { get; set; }

    }
}
