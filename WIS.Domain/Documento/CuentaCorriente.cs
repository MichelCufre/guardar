using System;

namespace WIS.Domain.Documento
{
    public class CuentaCorriente
    {
        public decimal NU_NIVEL { get; set; }
        public string NU_IDENTIFICADOR { get; set; }
        public decimal CD_FAIXA { get; set; }
        public string CD_PRODUTO { get; set; }
        public int CD_EMPRESA { get; set; }
        public string NU_DOCUMENTO_EGRESO { get; set; }
        public string CD_PRODUTO_PRODUCIDO { get; set; }
        public string NU_DOCUMENTO_EGRESO_PRDC { get; set; }
        public string TP_DOCUMENTO_EGRESO_PRDC { get; set; }
        public string TP_DOCUMENTO_INGRESO { get; set; }
        public string NU_DOCUMENTO_INGRESO { get; set; }
        public string TP_DOCUMENTO_INGRESO_ORIGINAL { get; set; }
        public string NU_DOCUMENTO_INGRESO_ORIGINAL { get; set; }
        public string TP_DOCUMENTO_EGRESO { get; set; }
        public decimal? QT_MOVIMIENTO { get; set; }
        public decimal? QT_DECLARADA_ORIGINAL { get; set; }
        public string NU_DOCUMENTO_CAMBIO { get; set; }
        public string TP_DOCUMENTO_CAMBIO { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public int? CD_FUNCIONARIO { get; set; }
    }
}
