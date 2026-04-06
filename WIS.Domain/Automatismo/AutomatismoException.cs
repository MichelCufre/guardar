using System;

namespace WIS.Domain.Automatismo
{
    public class AutomatismoException : Exception
    {
        public AutomatismoException() { }
        public AutomatismoException(string message) : base(message) { }
        public AutomatismoException(Exception ex) : base(ex.Message, ex.InnerException) { }
    }
}
