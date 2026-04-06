using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    [Table("V_REC500_DETALLE_POR_REFERENCIA")]
    public partial class V_REC500_DETALLE_POR_REFERENCIA
    {
        public int NU_RECEPCION_REFERENCIA { get; set; }

        [StringLength(20)]
        public string NU_REFERENCIA { get; set; }

        [StringLength(6)]
        public string TP_REFERENCIA { get; set; }

        public int CD_EMPRESA { get; set; }

        public DateTime? DT_ENTREGA { get; set; }

        public DateTime? DT_VENCIMIENTO_ORDEN { get; set; }
        public DateTime? DT_EMITIDA { get; set; }

        [StringLength(200)]
        public string DS_MEMO { get; set; }

        [StringLength(200)]
        public string VL_SERIALIZADO { get; set; }

        public int? CD_SITUACAO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_INTERFAZ_EJECUCION { get; set; }

        [StringLength(200)]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO3 { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(20)]
        public string ND_ESTADO_REFERENCIA { get; set; }

        [StringLength(6)]
        public string CD_MONEDA { get; set; }

        [StringLength(3)]
        public string TP_AGENTE { get; set; }

        [StringLength(40)]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        public string DS_AGENTE { get; set; }
    }
}
