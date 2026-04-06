using System;

namespace WIS.Domain.Documento.Serializables.Salida
{
    public class CambioDocInt
    {
        public CambioDocInt()
        {
            this.existeDocumento = false;
            this.success = true;
        }
        public string ID_PROCESADO { get; set; }
        public long NU_INTERFAZ_EJECUCION { get; set; }
        public string NU_REGISTRO { get; set; }
        public string CD_PRODUTO { get; set; }
        public decimal CD_FAIXA { get; set; }
        public string NU_IDENTIFICADOR { get; set; }
        public int CD_EMPRESA { get; set; }
        public string NU_DOCUMENTO { get; set; }
        public string TP_DOCUMENTO { get; set; }
        public string NU_DOCUMENTO_CAMBIO { get; set; }
        public string TP_DOCUMENTO_CAMBIO { get; set; }
        public Nullable<decimal> QT_CAMBIO { get; set; }
        public bool existeDocumento { get; set; }
        public bool existeDoc { get; set; }
        public bool success { get; set; }
        public int userId { get; set; }
        public string page { get; set; }
        public string errorMsg { get; set; }
    }
}
