using System;
using System.Globalization;

namespace WIS.TaskQueue.Providers
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
