namespace WIS.Configuration
{
    public class AuthSettings
    {
        public const string Position = "AuthSettings";
        public string AuthorizationApi { get; set; }
        public string TokenUrl { get; set; }
        public string AccessScope { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
