namespace WIS.TrafficOfficer
{
    public class ExpiredLicenseException : InvalidLicenseException
    {
        public ExpiredLicenseException() { }

        public ExpiredLicenseException(string message) : base(message) { }
    }
}
