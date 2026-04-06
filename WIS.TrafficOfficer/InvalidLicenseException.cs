using System;

namespace WIS.TrafficOfficer
{
    public class InvalidLicenseException : Exception
    {
        public InvalidLicenseException() { }

        public InvalidLicenseException(string message) : base(message) { }
    }
}
