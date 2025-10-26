namespace API.DTOs.Product
{
	public class UpdateSellingPriceRequest
	{
		public int ProductId { get; set; }
		public decimal NewPrice { get; set; }
	}
}
