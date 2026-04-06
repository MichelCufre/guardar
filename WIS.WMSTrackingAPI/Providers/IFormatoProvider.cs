using System;

namespace WIS.WMSTrackingAPI.Providers
{
    public interface IFormatoProvider
    {
        IFormatProvider FormatProvider { get; }
        public IFormatProvider GetFormatProvider();
    }
}