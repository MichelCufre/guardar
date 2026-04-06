namespace WIS.Domain.StockEntities.Constants
{
    public class TraspasoEmpresasDb
    {
        // Estados de traspaso
        public const string ESTADO_TRASPASO_EN_EDICION = "ESTRASP_EN_EDICION";
        public const string ESTADO_TRASPASO_EN_PROCESO = "ESTRASP_EN_PROCESO";
        public const string ESTADO_TRASPASO_FINALIZADO = "ESTRASP_FINALIZADO";
        public const string ESTADO_TRASPASO_CANCELADO = "ESTRASP_CANCELADO";

        // Configuraciones de pedido destino
        public const string MISMO_NUMERO = "CONFDEST_MISMO_NUM";
        public const string MISMO_CRITERIO = "CONFDEST_MISMO_CRIT";
        public const string ESPECIFICA_MANUALMENTE = "CONFDEST_ESPECIF_MANUAL";
    }
}
