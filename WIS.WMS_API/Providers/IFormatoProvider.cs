using System;

namespace WIS.WMS_API.Providers
{
    public interface IFormatoProvider
    {
        IFormatProvider FormatProvider { get; }
        public IFormatProvider GetFormatProvider();

    }
}