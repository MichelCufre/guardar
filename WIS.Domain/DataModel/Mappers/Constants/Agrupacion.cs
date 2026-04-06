using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class Agrupacion
    {
        //Dominio VLAGRUP
        public const string Ruta= "R";
        public const string Onda = "O";
        public const string Pedido= "P";
        public const string Cliente = "C";

        public static List<string> GetConstantNames()
        {
            List<string> names = new List<string>();
            Type type = typeof(Agrupacion);

            return type.GetFields().Select(x => x.GetValue(null).ToString()).ToList();
        }
    }
}
