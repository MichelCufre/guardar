using System.Collections.Generic;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class EstrategiaAlmacenajeReabastecimientoDb
    {
        //MODALIDAD_REABASTECIMIENTO

        public const string MINIMO = "MINIMO";
        public const string URGENTE = "URGENTE";
        public const string FORZADO = "FORZADO";

        public static List<string> GetModalidadesReabastecimiento()
        {
            List<string> colModalidades = new List<string>();

            colModalidades.Add(MINIMO);
            colModalidades.Add(URGENTE);
            colModalidades.Add(FORZADO);

            return colModalidades;
        }
    }
}
