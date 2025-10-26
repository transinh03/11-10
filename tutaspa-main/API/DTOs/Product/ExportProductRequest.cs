namespace API.DTOs.Product
{
	public class ExportProductRequest
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public string Note { get; set; }
	}
}
