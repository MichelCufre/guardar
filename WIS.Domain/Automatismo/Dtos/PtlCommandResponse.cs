namespace WIS.Domain.Automatismo.Dtos
{
    public class PtlCommandResponse
    {
        private const string SUCCESS = "SUCCESS";
        private const string ERROR = "ERROR";

        public PtlCommandResponse()
        {
            this.Status = SUCCESS;
        }

        public long CommandId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }

        public void SetError(string message)
        {
            this.Error = message;
            this.Status = ERROR;

        }

        public void SetSuccess(string message)
        {
            this.Message = message;
            this.Status = SUCCESS;
        }

        public bool IsSuccess()
        {
            return this.Status == SUCCESS;
        }
    }
}
