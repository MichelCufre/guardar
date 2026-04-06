namespace WIS.Automation
{
	public class PtlCommandResponse
	{

		private const string SUCCESS = "SUCCESS";
		private const string ERROR = "ERROR";

		public PtlCommandResponse()
		{
			Status = SUCCESS;
		}

		public long CommandId { get; set; }

		public string Status { get; set; }

		public string Message { get; set; }

		public string Error { get; set; }


		public void SetError(string message)
		{
			Error = message;
			Status = ERROR;
		}

		public void SetSuccess(string message)
		{
			Message = message;
			Status = SUCCESS;
		}

		public bool IsSuccess()
		{
			return Status == SUCCESS;
		}
	}
}
