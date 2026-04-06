namespace WIS.XmlData.WInterface.Models
{
    public enum ECSBackEndMethods
    {
        [StringEnumValue("PRUEBA_GetValorBackEnd")]
        PRUEBA_GetValorBackEnd,
        [StringEnumValue("BALANZA_GetValor")]
        BALANZA_GetValor,
        [StringEnumValue("BALANZA_WSK2B")]
        BALANZA_WSK2B,
        [StringEnumValue("BALANZA_WSK2B_CERO")]
        BALANZA_WSK2B_CERO,
        [StringEnumValue("AFTER_CierreAgenda")]
        AFTER_CierreAgenda,
        [StringEnumValue("AFTER_FacturarCamion")]
        AFTER_FacturarCamion,
        [StringEnumValue("GET_TipoRecepcionEmpresaPorDefecto")]
        GET_TipoRecepcionEmpresaPorDefecto,
        [StringEnumValue("GET_PrecioCompoentesBalanzaPorDefecto")]
        GET_PrecioCompoentesBalanzaPorDefecto,
        [StringEnumValue("GET_IngresoVehiculoCamposPorDefecto")]
        GET_IngresoVehiculoCamposPorDefecto,
    }
}
