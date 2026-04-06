using System;

namespace WIS.Exceptions
{
    public abstract class ExpectedException : Exception
    {
        public string[] StrArguments { get; set; }
        public string Property { get; set; }

        public string GetMessage()
        {
            if (this.StrArguments is null)
                return this.Message;

            return string.Format(this.Message, this.StrArguments);
        }

        public ExpectedException(string texto, string[] strArguments = null) : base(texto)
        {
            this.StrArguments = strArguments;
            this.Property = string.Empty;
        }
    }
}
