using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_RECEPCION_FACTURA_DET")]
    public partial class T_RECEPCION_FACTURA_DET
    {        
        [Key]
        public int NU_RECEPCION_FACTURA_DET { get; set; }
        
        public int NU_RECEPCION_FACTURA { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_FACTURADA { get; set; }

        public decimal? QT_VALIDADA { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public decimal? IM_UNITARIO_DIGITADO { get; set; }

        public DateTime? DT_VENCIMIENTO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        [StringLength(200)]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO4 { get; set; }

        public virtual T_RECEPCION_FACTURA T_RECEPCION_FACTURA { get; set; }

    }
}