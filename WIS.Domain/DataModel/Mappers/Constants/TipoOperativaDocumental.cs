namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class TipoOperativaDocumental
    {
        public const string Produccion = "PRO";
        public const string Transferencia = "TRA";

        public static string GetParamTpDocIngreso(string tpOperativa)
        {
            switch (tpOperativa)
            {
                case Produccion:
                    return ParamManager.TP_DOC_INGRESO_PRODUCCION;
                case Transferencia:
                    return ParamManager.TP_DOC_INGRESO_TRANSFERENCIA;
                default:
                    return null;
            }
        }

        public static string GetParamTpDocEgreso(string tpOperativa)
        {
            switch (tpOperativa)
            {
                case Produccion:
                    return ParamManager.TP_DOC_EGRESO_PRODUCCION;
                case Transferencia:
                    return ParamManager.TP_DOC_EGRESO_TRANSFERENCIA;
                default:
                    return null;
            }
        }
    }
}
