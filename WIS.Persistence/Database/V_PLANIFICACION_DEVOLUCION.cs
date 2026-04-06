using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("V_PLANIFICACION_DEVOLUCION")]
    public class V_PLANIFICACION_DEVOLUCION
    {
	    [Key]
	    [Column(Order = 0)]
	    public int NU_AGENDA { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        public int NU_CONTENEDOR { get; set; }

        [StringLength(40)]
        [Column]
        public string DS_CONTENEDOR { get; set; }

        [StringLength(50)]
        [Column]
        public string TP_BULTO { get; set; }

        public decimal? QT_AGENDADO { get; set; }

        public decimal? VL_CUBAGEM_TOTAL { get; set; }

        public decimal? VL_PESO_TOTAL { get; set; }

        public decimal? VL_ALTURA { get; set; }

        public decimal? VL_LARGURA { get; set; }

        public decimal? VL_PROFUNDIDADE { get; set; }


        [StringLength(50)]
        [Column]
        public string TP_REFERENCIA { get; set; }

        [StringLength(40)]
        public string CD_REFERENCIA { get; set; }


        [StringLength(10)]
        [Column]
        public string TP_CONTENEDOR { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_TELEFONO { get; set; }
        public DateTime? DT_PROMETIDA { get; set; }

    }
}
