using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_ORT_ORDEN_WORT030")]
    public partial class V_ORT_ORDEN_WORT030
    {

        public int? NU_ORT_ORDEN { get; set; }

        [StringLength(60)]
        public string DS_ORT_ORDEN { get; set; }

        public int? CD_FUNCIONARIO_ADDROW { get; set; }

        [StringLength(50)]
        public string LOGINNAME { get; set; }

        public DateTime? DT_INICIO { get; set; }

        public DateTime? DT_FIN { get; set; }

        public DateTime? DT_ULTIMA_OPERACION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(20)]
        public string ID_ESTADO { get; set; }

        public long? TOTAL_TAREA { get; set; }

        public long? TOTAL_TAREA_RESUELTA { get; set; }

        [StringLength(100)]
        public string DS_REFERENCIA { get; set; }

        public string PEDIDOS_INV { get; set; }

    }
}
