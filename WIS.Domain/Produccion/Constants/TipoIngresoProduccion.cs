namespace WIS.Domain.Produccion.Constants
{
    public class TipoIngresoProduccion
    {
        public const string PanelWeb = "TPINGPRPAN";
        public const string Colector = "TPINGPRCOL";
        public const string BlackBox = "TPINGPR_BLACKBOX";
        public const string WhiteBox = "TPINGPR_WHITEBOX";

        public const string MOTIVO_PRODUCCION = "MOT_PROD";
        public const string MOTIVO_CONSUMO = "MOT_CONS";

        //Producción

        public const string MOTIVO_PROD = "PRO";
        public const string MOTIVO_SOBRANTE = "SOB";
        public const string MOTIVO_PRODUCCION_AJUSTE_STOCK = "PAS";

        public const string MOT_PROD_PRO = "MOT_PROD_PRO";
        public const string MOT_PROD_SOB = "MOT_PROD_SOB";
        public const string MOT_PROD_ADS = "MOT_PROD_ADS";

        //Consumición

        public const string MOTIVO_CONSUMICION_AJUSTE_STOCK = "CAS";
        public const string MOTIVO_CONSUMICION_CONSUMO = "CON";

        public const string MOT_CONS_ADS = "MOT_CONS_ADS";
        public const string MOT_CONS_CON = "MOT_CONS_CON";
    }
}
