using System.Runtime.InteropServices.Marshalling;
using WIS.FormComponent;

namespace WIS.Domain.DataModel.Mappers
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

        public string NullIfEmpty(string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        public static string MapStringToBooleanString(string value)
        {
            return (bool.Parse(value) ? "S" : "N");
        }
    }
}
