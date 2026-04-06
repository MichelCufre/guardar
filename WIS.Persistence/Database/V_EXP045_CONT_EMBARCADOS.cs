using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_EXP045_CONT_EMBARCADOS")]
    public partial class V_EXP045_CONT_EMBARCADOS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_CONTENEDOR { get; set; }

        public int? CD_CAMION { get; set; }

        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        public string DS_CLIENTE { get; set; }

        [StringLength(100)]
        public string DS_TIPO_AGENTE { get; set; }

        [StringLength(40)]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        public string DS_AGENTE { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        public int? QT_BULTO { get; set; }

        [StringLength(10)]
        public string TP_CONTENEDOR { get; set; }

        [StringLength(20)]
        public string ID_PRECINTO_1 { get; set; }

        [StringLength(20)]
        public string ID_PRECINTO_2 { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [StringLength(1)]
        public string VL_CONTROL { get; set; }

		[StringLength(50)]
		public string ID_EXTERNO_CONTENEDOR { get; set; }

        [StringLength(100)]
        public string CD_BARRAS_CONTENEDOR { get; set; }

        public long? NU_LPN { get; set; }

        public int? NU_UNIDAD_TRANSPORTE { get; set; }

        [StringLength(40)]
        public string NU_EXTERNO_UNIDAD { get; set; }

        [StringLength(3)]
        public string TP_UNIDAD_TRANSPORTE { get; set; }

        [StringLength(50)]
        public string CD_BARRAS_UT { get; set; }

        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

    }
}
