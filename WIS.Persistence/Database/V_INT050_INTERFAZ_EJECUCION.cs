using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_INT050_INTERFAZ_EJECUCION")]
    public partial class V_INT050_INTERFAZ_EJECUCION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }
        
        public int? CD_FUNCIONARIO_ACEPTACION { get; set; }
        
        public int? CD_INTERFAZ_EXTERNA { get; set; }

        [StringLength(100)]
        public string DS_INTERFAZ_EXTERNA { get; set; }

        [StringLength(6)]
        public string TP_ARCHIVO { get; set; }
        
        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(100)]
        public string NM_ARCHIVO { get; set; }

        [StringLength(200)]
        public string DS_REFERENCIA { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(30)]
        public string ND_SITUACION { get; set; }

        [StringLength(30)]
        public string DS_SITUACAO { get; set; }

        public DateTime? DT_COMIENZO { get; set; }

        public DateTime? DT_SITUACAO { get; set; }

        [StringLength(44)]
        public string DURACION { get; set; }

        [StringLength(1)]
        public string FL_ERROR_CARGA { get; set; }

        [StringLength(1)]
        public string FL_ERROR_PROCEDIMIENTO { get; set; }

        [StringLength(1)]
        public string ID_ENTRADA_SALIDA { get; set; }

        public int CD_INTERFAZ { get; set; }

        [StringLength(60)]
        public string NM_PROCEDIMIENTO { get; set; }

        public long? COUNT_ERRORS { get; set; }

        [StringLength(20)]
        public string CD_GRUPO_CONSULTA { get; set; }

        [StringLength(100)]
        public string DS_GRUPO_CONSULTA { get; set; }

		[StringLength(50)]
		public string ID_REQUEST { get; set; }

		public int? USERID { get; set; }

		[StringLength(100)]
		public string FULLNAME { get; set; }

        [StringLength(100)]
        public string VL_ENDPOINT { get; set; }

        [StringLength(100)]
        public string VL_ENDPOINT_REPROCESS { get; set; }

        [StringLength(100)]
        public string VL_PARAMETRO_HABILITACION { get; set; }
    }
}
