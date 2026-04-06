using System;
using System.Globalization;

namespace WIS.WMS_API.Providers
{
    public class FormatoProvider : IFormatoProvider
    {
        public IFormatProvider FormatProvider { get; }

        public FormatoProvider()
        {
            FormatProvider = CultureInfo.InvariantCulture;
        }

        public IFormatProvider GetFormatProvider()
        {
            return this.FormatProvider;
        }
    }
}
