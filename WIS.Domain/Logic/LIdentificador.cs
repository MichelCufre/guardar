using System.Text.RegularExpressions;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Logic
{
    public class LIdentificador
    {
        public static bool ContieneCaracteresNoPermitidos(IUnitOfWork uow, string identificador)
        {
            var lovSeparator = uow.ParametroRepository.GetParameter(ParamManager.MOBILE_LOV_ID_SEPARATOR);
            var caracteresNoPermitidos = (CSystem.ID_SEPARATOR + lovSeparator + uow.ParametroRepository.GetParameter(ParamManager.CARACTERES_NO_PERMITIDOS_LOTE));

            return ContieneCaracteresNoPermitidos(identificador, caracteresNoPermitidos);
        }

        public static bool ContieneCaracteresNoPermitidos(string identificador, string caracteresNoPermitidos)
        {
            return Regex.IsMatch(identificador, $"[{Regex.Escape(caracteresNoPermitidos)}]");
        }
    }
}
