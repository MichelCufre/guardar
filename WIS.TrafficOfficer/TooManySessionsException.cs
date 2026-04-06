using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.TrafficOfficer
{
    public class TooManySessionsException : Exception
    {
        public TooManySessionsException() { }

        public TooManySessionsException(string message) : base(message) { }
    }
}
