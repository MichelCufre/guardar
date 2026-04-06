namespace WIS.Domain.ManejoStock.Constants
{
    public class TiposMovimiento
    {
        public const string INGRESOBB = "MOV_ENTRADA_IN";
        public const string RECHAZO_SANO = "MOV_REC_SANO_IN";
        public const string RECHAZO_AVERIA = "MOV_REC_AVER_IN";
        public const string CONSUMO = "MOV_CONSUMO";
        public const string PRODUCIR = "MOV_PRODUCIR";
        public const string SALIDA_PRODUCTO = "MOV_SAL_OUT";
        public const string SALIDA_INSUMO = "MOV_SAL_INS_OUT";
        public const string SALIDA_PRODUCTO_AVERIADO = "MOV_SAL_AVE_OUT";
        public const string SALIDA_INSUMO_AVERIADO = "MOV_SAL_INS_AVE_OUT";
        public static string SALIDA_INSUMO_SEMIACABADO = "MOV_CONSUMO_SEMI";
        public static string SALIDA_INSUMO_CONSUMIBLE = "MOV_CONSUMO_CONS";

        //Dominio - TMOV
        public const string Expedición = "EXP";
        public const string Recepcion = "RECE";
        public const string Traslado = "TRAS";
        public const string AlmacenamientoFraccionado = "ALMF";
        public const string AlmacenamientoAgrupado = "ALMA";
        public const string Moviles = "MOVI";
        public const string AjusteCajas = "CAJA";
        public const string AjusteInventario = "INVE";
        public const string AjusteVerificacionBultos = "VERBUL";
        public const string AjusteEtiqueta = "AJET";
        public const string Preparación = "PRE";
        public const string TransferirEtiqueta = "TRET";
        public const string Clasificacion = "CLAS";
    }
}
