namespace WIS.TrafficOfficer
{
    public class ExpiredPageTokenException : InvalidLicenseException
    {
        public ExpiredPageTokenException() { }

        public ExpiredPageTokenException(string message) : base(message) { }
    }
}
