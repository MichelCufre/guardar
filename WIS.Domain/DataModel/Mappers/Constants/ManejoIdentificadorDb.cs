using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class ManejoIdentificadorDb
    {
        public const string Producto = "P";
        public const string Lote = "L";
        public const string Serie = "S";

        public const string IdentificadorProducto = "*";
        public const string IdentificadorAuto = "(AUTO)";
        public const string IdentificadorVarios = "VARIOS";

        public static List<string> GetConstantNames()
        {
            List<string> names = new List<string>();
            Type type = typeof(ManejoIdentificadorDb);

            return type.GetFields().Where(x => x.Name != "IdentificadorProducto" && x.Name != "IdentificadorAuto").Select(x => x.GetValue(null).ToString()).ToList();
        }
    }
}
