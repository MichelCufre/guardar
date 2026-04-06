namespace WIS.Automation.Galys
{
    public class GalysResponse
    {
        public string descError { get; set; }

        public string msg_success { get; set; }

        public int resultado { get; set; }

        public void SetError(string message)
        {
            descError = message;
            resultado = 99;
        }
    }
}
