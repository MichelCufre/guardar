namespace WIS.Domain.Automatismo.Constants
{
    public class CodigoInterfazAutomatismoDb
    {
        public const int CD_INTERFAZ_PRODUCTOS = 1500;
        public const int CD_INTERFAZ_CODIGO_BARRAS = 1550;
        public const int CD_INTERFAZ_ENTRADAS = 1600;
        public const int CD_INTERFAZ_SALIDA = 1700;
        public const int CD_INTERFAZ_UBICACIONES_PICKING = 1800;
        public const int CD_INTERFAZ_NOTIF_AJUSTES = 2500;
        public const int CD_INTERFAZ_CONF_ENTRADAS = 2600;
        public const int CD_INTERFAZ_CONF_SALIDAS = 2700;
        public const int CD_INTERFAZ_CONF_MOVIMIENTO = 3000;

        #region AUTOMATISMO - PTL - PRONTOMETAL
        public const short TrunOnLigth = 555;
        public const short StartOfOperation = 666;
        public const short ResetOfOperation = 777;
        public const short ConfirmCommand = 888;
        #endregion

        #region AUTOMATISMO - PTL - SMARTLOG
        public const short TrunOnLigth_Smarlog = 5551;
        public const short StartOfOperation_Smarlog = 6661;
        public const short ResetOfOperation_Smarlog = 7771;
        public const short ConfirmCommand_Smarlog = 8881;
        #endregion
    }
}
