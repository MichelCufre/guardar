namespace WIS.AutomationInterpreter.Models
{
    public class IntegrationSettings
    {
        public const string Position = "IntegrationSettings";

        public string UrlIntegracion { get; set; }

        public bool Habilitado { get; set; }

        public string TipoAutenticacion { get; set; }

        public string TipoComunicacion { get; set; }

        public string User { get; set; }

        public string Secret { get; set; }

        public string Scope { get; set; }

        public string UrlAuthServer { get; set; }
    }
}
