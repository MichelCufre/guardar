namespace WIS.Domain.Documento.Serializables.Salida
{
    public class CambioDocumentoDetIngreso
    {

        public CambioDocumentoDetIngreso()
        {
            this.existeDocumento = false;
            this.success = true;
        }
        public bool existeDoc { get; set; }
        public string TP_DOCUMENTO_NUEVO { get; set; }
        public string NU_DOCUMENTO_NUEVO { get; set; }
        public bool existeDocumento { get; set; }
        public bool success { get; set; }
        public string NU_DOCUMENTO_EGRESO { get; set; }
        public string TP_DOCUMENTO_EGRESO { get; set; }
        public string NU_DOCUMENTO_EGRESO_PRDC { get; set; }
        public string TP_DOCUMENTO_EGRESO_PRDC { get; set; }
        public string TP_DOCUMENTO_INGRESO { get; set; }
        public string NU_DOCUMENTO_INGRESO { get; set; }
        public string TP_DOCUMENTO_INGRESO_ORIGINAL { get; set; }
        public string NU_DOCUMENTO_INGRESO_ORIGINAL { get; set; }
        public int CD_EMPRESA { get; set; }
        public string CD_PRODUTO { get; set; }
        public decimal CD_FAIXA { get; set; }
        public string NU_IDENTIFICADOR { get; set; }
        public string CD_PRODUTO_PRODUCIDO { get; set; }
        public int NU_NIVEL { get; set; }
        public decimal QT_MOVIMIENTO { get; set; }
        public int userId { get; set; }
        public string page { get; set; }
        public string errorMsg { get; set; }
    }
}
