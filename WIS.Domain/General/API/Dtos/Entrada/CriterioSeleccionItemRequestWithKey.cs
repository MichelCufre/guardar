namespace WIS.Domain.General.API.Dtos.Entrada
{
	public class CriterioSeleccionItemRequestWithKey : CriterioSeleccionItemRequest
	{
		public int CodigoControlCalidad { get; set; }
		public string Estado { get; set; } // "CTRASC" || "CTRAPR"
	}
}
