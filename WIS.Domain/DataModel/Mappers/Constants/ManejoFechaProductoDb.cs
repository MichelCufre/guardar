using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class ManejoFechaProductoDb
    {
        /// <summary>
        /// first in first out
        /// </summary>
        public const string Fifo = "F";
        public const string Duradero = "D";
        /// <summary>
        /// FEFO
        /// </summary>
        public const string Expirable = "E";

        public static List<string> GetConstantNames()
        {
            List<string> names = new List<string>();
            Type type = typeof(ManejoFechaProductoDb);

            return type.GetFields().Select(x => x.GetValue(null).ToString()).ToList();
        }
    }
}
