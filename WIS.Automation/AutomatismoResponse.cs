namespace WIS.Automation
{
	public class AutomatismoResponse
	{
		public long Operacion { get; set; }
		public string Mensaje { get; set; }
		public bool IsError { get; set; }

		public void SetError(string msg)
		{
			this.Mensaje = msg;
			this.IsError = true;
		}
	}
}
