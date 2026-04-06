using System;

namespace WIS.TaskQueue.Providers
{
    public interface IFormatoProvider
    {
        IFormatProvider FormatProvider { get; }
        public IFormatProvider GetFormatProvider();

    }
}