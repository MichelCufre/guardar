using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.WMS_API.Models.Mappers
{
    public abstract class Mapper
    {
        public bool MapStringToBoolean(string value)
        {
            return !string.IsNullOrEmpty(value) && value == "S";
        }

        public string MapBooleanToString(bool value)
        {
            return value ? "S" : "N";
        }

        public bool MapShortToBoolean(short value)
        {
            return value == 1;
        }

        public short MapBooleanToShort(bool value)
        {
            return (short)(value ? 1 : 0);
        }
    }
}
