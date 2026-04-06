using System.Collections.Generic;
using System;
using System.Linq;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class TipoOperacionReferencia
    {
        public const string Nuevo = "N";
        public const string Anular = "A";
        public const string Modificar = "M";
        public const string Reemplazar = "R";

        public static List<string> GetConstantNames()
        {
            List<string> names = new List<string>();
            Type type = typeof(TipoOperacionReferencia);

            return type.GetFields().Select(x => x.GetValue(null).ToString()).ToList();
        }
    }
}
