
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("T_LPN_TIPO")]
    public partial class T_LPN_TIPO
    {

		[Key]
		[Column(Order = 0)]
		[StringLength(10)]
		public string TP_LPN_TIPO { get; set; }

		[Required]
		[StringLength(30)]
		public string NM_LPN_TIPO { get; set; }

		[StringLength(400)]
		public string DS_LPN_TIPO { get; set; }

		[Required]
		[StringLength(1)]
		public string FL_PERMITE_CONSOLIDAR { get; set; }

		[Required]
		[StringLength(1)]
		public string FL_PERMITE_EXTRAER_LINEAS { get; set; }

		[Required]
		[StringLength(1)]
		public string FL_PERMITE_AGREGAR_LINEAS { get; set; }

		[Required]
		[StringLength(1)]
		public string FL_CREAR_SOLO_AL_INGRESO { get; set; }

		[Required]
		[StringLength(1)]
		public string FL_MULTIPRODUCTO { get; set; }

		[Required]
		[StringLength(1)]
		public string FL_MULTI_LOTE { get; set; }

		[Required]
		[StringLength(1)]
		public string FL_PERMITE_ANIDACION { get; set; }

		[StringLength(15)]
		public string NU_TEMPLATE_ETIQUETA { get; set; }

		[StringLength(10)]
		public string NU_COMPONENTE { get; set; }

		[StringLength(1)]
		public string FL_CONTENEDOR_LPN { get; set; }

		public long? NU_SEQ_LPN { get; set; }

		[StringLength(1)]
		public string FL_PERMITE_GENERAR { get; set; }

		[StringLength(1)]
		public string FL_INGRESO_RECEPCION_ATRIBUTO { get; set; }

		[StringLength(1)]
		public string FL_INGRESO_PICKING_ATRIBUTO { get; set; }

		[StringLength(20)]
		public string VL_PREFIJO { get; set; }

		[StringLength(20)]
		public string TP_ETIQUETA_RECEPCION { get; set; }

		[StringLength(1)]
		public string FL_PERMITE_DESTRUIR_ALM { get; set; }
		public virtual T_LPN T_LPN { get; set; }

		public decimal? VL_PICKING_SCORE_EQNR { get; set; }

        public decimal? VL_PICKING_SCORE_EQR { get; set; }

        public decimal? VL_PICKING_SCORE_LTNR { get; set; }

        public decimal? VL_PICKING_SCORE_LTR { get; set; }

        public decimal? VL_PICKING_SCORE_GT { get; set; }

        public decimal? VL_PICKING_SCORE_NE { get; set; }

        public decimal? VL_PICKING_SCORE_BONUS { get; set; }
    }
}
