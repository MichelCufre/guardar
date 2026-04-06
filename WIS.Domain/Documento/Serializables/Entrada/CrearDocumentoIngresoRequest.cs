using System.Collections.Generic;

namespace WIS.Domain.Documento.Serializables.Entrada
{
    public class CrearDocumentoIngresoRequest
    {
        public string USER { get; set; }
        public string APLICACION { get; set; }
        public string NU_DOCUMENTO { get; set; }
        public string TP_DOCUMENTO { get; set; }
        public string CD_EMPRESA { get; set; }
        public string NU_IMPORT { get; set; }
        public string NU_EXPORT { get; set; }
        public string CD_DESPACHANTE { get; set; }
        public string NU_FACTURA { get; set; }
        public string NU_CONOCIMIENTO { get; set; }
        public string QT_BULTO { get; set; }
        public string CD_UNIDAD_MEDIDA_BULTO { get; set; }
        public string QT_PESO { get; set; }
        public string QT_CONTENEDOR20 { get; set; }
        public string QT_CONTENEDOR40 { get; set; }
        public string TP_ALMACENAJE_Y_SEGURO { get; set; }
        public string CD_VIA { get; set; }
        public string CD_TRANSPORTISTA { get; set; }
        public string DT_PROGRAMADO { get; set; }
        public string QT_VOLUMEN { get; set; }
        public string VL_ARBITRAJE { get; set; }
        public string DS_DOCUMENTO { get; set; }
        public string DS_ANEXO1 { get; set; }
        public string DS_ANEXO2 { get; set; }
        public string DS_ANEXO3 { get; set; }
        public string DS_ANEXO4 { get; set; }
        public string DS_ANEXO5 { get; set; }
        public string NU_PREDIO { get; set; }
        public string CD_AGENTE { get; set; }
        public string TP_AGENTE { get; set; }
        public string CD_MONEDA { get; set; }
        public string VL_SEGURO { get; set; }
        public string VL_FLETE { get; set; }
        public string VL_OTROS_GASTOS { get; set; }
        public string ICMS { get; set; }
        public string II { get; set; }
        public string IPI {get;set;}
        public string IISUSPENSO { get; set; }
        public string IPISUSPENSO { get; set; }
        public string PISCONFINS { get; set; }
        public string CD_REGIMEN_ADUANA { get; set; }
        public List<CrearDetalleIngresoRequest> DETALLES { get; set; }
    }

    public class CrearDetalleIngresoRequest
    {
        public string CD_PRODUTO { get; set; }
        public string NU_IDENTIFICADOR { get; set; }
        public string QT_INGRESADA { get; set; }
        public string VL_MERCADERIA { get; set; }
        public string VL_TRIBUTO { get; set; }
        public string CD_FAIXA { get; set; }
        public string NU_REGISTRO { get; set; }
    }
}
