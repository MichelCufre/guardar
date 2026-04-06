namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRDC_INGRESO_KIT110")]
    public partial class V_PRDC_INGRESO_KIT110
    {
		[Key]
		[StringLength(10)]
		public string NU_PRDC_INGRESO { get; set; }

		[StringLength(10)]
		public string CD_PRDC_DEFINICION { get; set; }

		public int? QT_FORMULA { get; set; }

		public string ID_GENERAR_PEDIDO { get; set; }

		public int? CD_FUNCIONARIO { get; set; }

		public short? CD_SITUACAO { get; set; }

		[StringLength(30)]
		public string DS_SITUACAO { get; set; }

		public DateTime? DT_ADDROW { get; set; }

		public DateTime? DT_UPDROW { get; set; }

		[StringLength(10)]
		public string NU_PRDC_ORIGINAL { get; set; }

		[StringLength(200)]
		public string DS_ANEXO1 { get; set; }

		[StringLength(200)]
		public string DS_ANEXO2 { get; set; }

		[StringLength(200)]
		public string DS_ANEXO3 { get; set; }

		[StringLength(200)]
		public string DS_ANEXO4 { get; set; }

		public long? NU_INTERFAZ_EJEC_ENT { get; set; }

		[StringLength(20)]
		public string ND_TIPO { get; set; }

		[StringLength(10)]
		public string CD_PRDC_LINEA { get; set; }

		[StringLength(60)]
		public string MODALIDAD_TRABAJO { get; set; }

		[StringLength(10)]
		public string NU_PREDIO { get; set; }

		[StringLength(200)]
		public string DS_ANEXO5 { get; set; }

		[StringLength(100)]
		public string ID_PROD_EXT { get; set; }

		[StringLength(40)]
		public string NU_LOTE { get; set; }

		[StringLength(100)]
		public string CD_PRD_INSU_ANCLA { get; set; }

		public int? NU_POSICION_EN_COLA { get; set; }

		[StringLength(20)]
		public string ND_ASIGNACION_LOTE { get; set; }

		public string FL_PERMITIR_AUTO_LINEA { get; set; }

		public string FL_ING_DIRECTO_A_PROD { get; set; }

		public int? CD_EMPRESA { get; set; }

		public long? NU_TRANSACCION { get; set; }

		[StringLength(30)]
		public string NM_FUNCIONARIO { get; set; }

        public DateTime? DT_INICIO_PRODUCCION { get; set; }

        public DateTime? DT_FIN_PRODUCCION { get; set; }

        public string ID_MANUAL { get; set; }

        public string DS_TIPO { get; set; }

        public string DS_ASIGNACION_LOTE { get; set; }

        public long? NU_ULT_INTERFAZ_EJECUCION { get; set; }

        [StringLength(40)]
        public string TP_FLUJO { get; set; }

        

    }
}
