namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class BarcodeDb
    {
        //Modulo
        public const int NUM_MODULO_CON_W = 10;
        public const int NUM_MODULO_CON_C = 10;
        public const int NUM_MODULO_CON_T = 11;
        public const int NUM_MODULO_CON_D = 10;
        public const int NUM_MODULO_ET_GRA = 10;
        public const int NUM_MODULO_ET_CHI = 10;
        public const int NUM_MODULO_ET_BUL = 10;
        public const int NUM_MODULO_ET_MDA = 10;
        public const int NUM_MODULO_ET_VIR = 10;

        public const int NUM_MODULO_PALLET = 10;
        public const int NUM_MODULO_RECEPCION = 10;
        public const int NUM_MODULO_TRANFERENCIA = 10;
        public const int NUM_MODULO_EQUIPO = 10;
        public const int NUM_MODULO_CON_UT = 10;

        //Largo
        public const int LENGHT_MIN_CB_CONT = 5;
        public const int LENGHT_MIN_CB_UBI = 6;
        public const int LENGHT_MIN_CB_TRA = 6;

        public const int LENGHT_RECEPCION = 10;
        public const int LENGHT_TRANFERENCIA = 10;
        public const int LENGHT_PALLET = 10;

        public const int LENGHT_CON_W = 10;
        public const int LENGHT_CON_C = 10;
        public const int LENGHT_CON_V = 10;
        public const int LENGHT_CON_T = 8;
        public const int LENGHT_CON_D = 10;
        public const int LENGHT_UBICEQUIPO_FUN = 5;
        public const int LENGHT_UBICEQUIPO_QUI = 5;
        public const int LENGHT_EQUIPO = 8;
        public const int LENGHT_ET_GRA = 6;
        public const int LENGHT_ET_CHI = 6;
        public const int LENGHT_ET_BUL = 6;
        public const int LENGHT_ET_MDA = 6;
        public const int LENGHT_ET_VIR = 6;

        //Tipo 
        public const string TIPO_CONTENEDOR = "TP_CONTENEDOR";
        public const string TIPO_CONTENEDOR_W = "W";
        public const string TIPO_CONTENEDOR_T = "T";
        public const string TIPO_CONTENEDOR_D = "D";
        public const string TIPO_CONTENEDOR_C = "C";
        public const string TIPO_CONTENEDOR_V = "V";

        public const string TIPO_CONTENEDOR_GRANDE = "1";
        public const string TIPO_CONTENEDOR_CHICO = "2";
        public const string TIPO_CONTENEDOR_BULTO = "3";
        public const string TIPO_CONTENEDOR_MEDIANO = "4";
        public const string TIPO_CONTENEDOR_VIRTUAL = "5";

        public const string TIPO_ET_UT = "UT";
        public const string TIPO_ET_RECEPCION = "REC";
        public const string TIPO_ET_TRANFERENCIA = "TRA";
        public const string TIPO_ET_PALLET = "PAL";
        public const string TIPO_ET_UBICEQUIPO_FUN = "EQF";
        public const string TIPO_ET_UBICEQUIPO_EQI = "EQI";
        public const string TIPO_ET_EQUIPO = "EQ";
        public const string TIPO_ET_EQUIPO_MANUAL = "EQUIP";
        public const string TIPO_ET_CON_W = "CONT_W";
        public const string TIPO_ET_CON_C = "CONT_C";
        public const string TIPO_ET_CON_T = "CONT_T";
        public const string TIPO_ET_CON_D = "CONT_D";
        public const string TIPO_ET_CON_V = "CONT_V";

        public const string TIPO_ET_CON_GRA = "CONT_GRA";
        public const string TIPO_ET_CON_CHI = "CONT_CHI";
        public const string TIPO_ET_CON_BUL = "CONT_BUL";
        public const string TIPO_ET_CON_MED = "CONT_MED";
        public const string TIPO_ET_CON_VIR = "CONT_VIR";

        public const string ID_CLIENTE_PREDEFINIDO_S = "S";
        public const string ID_CLIENTE_PREDEFINIDO_N = "N";

        public const string FLG_CONSOLIDACION_MASIVA = "CONSOLIDACION MASIVA";

        public const string TIPO_ET_ENTRADA = "EE";
        public const string TIPO_ET_CONTENEDOR = "CO";
        public const string TIPO_ET_TRANSFERENCIA = "ET";
        public const string TIPO_ET_PRODUCTO = "EP";
        public const string TIPO_ET_UBICACION = "UB";
        public const string TIPO_ET_SALIDA = "ES";

        public const string TP_ETIQUETA_REC = "WE";
        public const string TIPO_ET_UTR = "UTR";
        public const string TP_ETIQUETA_CONT = "WC";

        public const int InternoWis = 0;
        public const string TIPO_LPN_CB = "CB";
        public const string TIPO_LPN_RF = "RF";
        //Prefix
        public const string PREFIX_PALLET = "WISPA";
        public const string PREFIX_TRANSFERENCIA = "WISET";
        public const string PREFIX_RECEPCION = "WISEL";
        public const string PREFIX_CONT_TIPO_W = "WISCO";
        public const string PREFIX_CONT_TIPO_C = "WISCC";
        public const string PREFIX_CONT_TIPO_T = "PP";
        public const string PREFIX_CONT_TIPO_D = "";
        public const string PREFIX_UBICEQUIPO_TIPO_FUNC = "FUNC";
        public const string PREFIX_UBICEQUIPO_TIPO_EQI = "ZE";
        public const string PREFIX_EQUIPO = "WISEQ";
        public const string PREFIX_UT = "WISUT";
        public const string PREFIX_CONT_TIPO_V = "WISVV";
        public const string PREFIX_RECEPCIONX = "WISEX";
        public const string PREFIX_UBICACION = "WISUB";
        public const string PREFIX_UBICACION_WISSD = "WISSD";
        public const string PREFIX_UBICACION_PARCIAL = "WISUP";
        public const string PREFIX_CONT_TIPO_UTR = "WISUT";

        public const string PREFIX_CONT_TIPO_GRA = "";
        public const string PREFIX_CONT_TIPO_CHI = "";
        public const string PREFIX_CONT_TIPO_MED = "";
        public const string PREFIX_CONT_TIPO_VIR = "";
        public const string PREFIX_CONT_TIPO_BUL = "";
        public const string PREFIX_UBIC_CLASIFICACION = "CLASIF";


        //Padding value
        public const char PADDING_CHAR = '0';

        //Tags
        public const string COMANDO_WIS_OP = "<WIS>";
        public const string COMANDO_WIS_CL = "</WIS>";
        public const string COMANDO_TEXTO_OP = "<TEXTO>";
        public const string COMANDO_TEXTO_CL = "</TEXTO>";
        public const string COMANDO_CAMPO_OP = "<CAMPO>";
        public const string COMANDO_CAMPO_CL = "</CAMPO>";
        public const string COMANDO_LARGO_OP = "<LARGO>";
        public const string COMANDO_LARGO_CL = "</LARGO>";

        public const string PREFIX_UBIC_PRODUCCION = "PRD-";
    }
}
