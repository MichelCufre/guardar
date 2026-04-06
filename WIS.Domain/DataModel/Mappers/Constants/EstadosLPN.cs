using System.Collections.Generic;
using WIS.Domain.General;
using WIS.Domain.ManejoStock.Constants;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class EstadosLPN
    {
        public const string Generado = "LPNGEN";
        public const string Importado = "LPNIMP";
        public const string Activo = "LPNACT";
        public const string Bloqueado = "LPNBLO";
        public const string Finalizado = "LPNFIN";
        public const string Contenedor = "LPNCON";

        public static List<string> GetEstadosPermitidos(string tipoOperacion)
        {
            switch (tipoOperacion)
            {
                case TipoOperacionDb.Inventario:
                    return new List<string> { Generado, Activo, Finalizado };
                default:
                    return new List<string> { Generado, Importado, Activo, Bloqueado, Finalizado, Contenedor };
            }
        }
    }
}
