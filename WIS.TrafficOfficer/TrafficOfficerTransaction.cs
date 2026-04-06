namespace WIS.TrafficOfficer
{
    public class TrafficOfficerTransaction
    {
        public string Token { get; private set; }

        public TrafficOfficerTransaction(string token)
        {
            this.Token = token;
        }

        public bool HasToken()
        {
            return !string.IsNullOrEmpty(this.Token);
        }
    }
}
