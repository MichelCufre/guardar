
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("T_AUTOMATISMO_CONF_ENTRADA")]
    public partial class T_AUTOMATISMO_CONF_ENTRADA
    {

        [Key]
        [Column(Order = 0)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(30)]
        public string NU_INTERFAZ_EJECUCION_ENT { get; set; }

        [StringLength(1)]
        public string FL_ULT_OPERACION { get; set; }

        [Key]
        [Column(Order = 3)]
        public int CD_EMPRESA { get; set; }

        [StringLength(200)]
        public string DS_REFERENCIA { get; set; }

        [StringLength(100)]
        public string NM_ARCHIVO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO_DEST { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 9)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        [StringLength(1)]
        public string FL_ULT_OPERACION_DET { get; set; }

        [StringLength(50)]
        public string LOGINNAME { get; set; }

		[Key]
		[Column(Order = 10)]
		public int ID_LINEA_ENTRADA { get; set; }
	}
}
