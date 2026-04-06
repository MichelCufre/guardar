using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_TAREAS_WORT120")]
    public partial class V_TAREAS_WORT120
    {
        public long NU_ORDEN_TAREA { get; set; }

        [Required]
        [StringLength(10)]
        public string CD_TAREA { get; set; }

        public int NU_ORT_ORDEN { get; set; }

        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(1)]
        public string FL_RESUELTA { get; set; }

        public DateTime? DT_INICIO { get; set; }

        public DateTime? DT_FIN { get; set; }

        public DateTime? DT_ULTIMA_OPERACION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(20)]
        public string ID_ESTADO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(60)]
        public string DS_MEMO { get; set; }

        public DateTime? DT_DESDE { get; set; }

        public DateTime? DT_HASTA { get; set; }

        public decimal QT_INSUMO_MANIPULEO { get; set; }

        [StringLength(100)]
        public string DS_REFERENCIA { get; set; }

        [Required]
        [StringLength(10)]
        public string CD_INSUMO_MANIPULEO { get; set; }

        [StringLength(60)]
        public string DS_INSUMO_MANIPULEO { get; set; }

        [StringLength(60)]
        public string DS_TAREA { get; set; }

        [Required]
        [StringLength(6)]
        public string TP_TAREA { get; set; }

        [StringLength(100)]
        public string DS_REFERENCIA_ORDEN { get; set; }

        [StringLength(143)]
        public string PEDIDOS_INV { get; set; }
    }
}