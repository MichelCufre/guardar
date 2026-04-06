using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    [Table("V_REC500_FACTURAS")]
    public partial class V_REC500_FACTURAS
    {

        [Key]
        [Column(Order = 0)]
        public int NU_RECEPCION_FACTURA { get; set; }

        [StringLength(3)]
        public string NU_SERIE { get; set; }

        [StringLength(12)]
        public string NU_FACTURA { get; set; }

        [StringLength(12)]
        public string TP_FACTURA { get; set; }

        public int CD_EMPRESA { get; set; }

        public int? NU_AGENDA { get; set; }

        public DateTime? DT_EMISION { get; set; }

        public decimal? IM_TOTAL_DIGITADO { get; set; }

        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(1)]
        public string ID_ORIGEN { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_VENCIMIENTO { get; set; }

        [StringLength(6)]
        public string CD_MONEDA { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [StringLength(200)]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        public string DS_OBSERVACION { get; set; }

        [StringLength(20)]
        public string ND_ESTADO { get; set; }

        [StringLength(20)]
        public string NU_REFERENCIA { get; set; }

        [StringLength(40)]
        public string NU_REMITO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        [StringLength(40)]
        public string CD_AGENTE { get; set; }   

        [StringLength(100)]
        public string DS_AGENTE { get; set; }

        [StringLength(3)]
        public string TP_AGENTE { get; set; }
        

    }
}
