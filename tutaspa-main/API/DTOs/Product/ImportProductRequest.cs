namespace API.DTOs.Product
{
	public class ImportProductRequest
	{
		public string ProductName { get; set; }
		public string Description { get; set; }
		public int CategoryId { get; set; }
		public decimal CurrentSellingPrice { get; set; }
		public string SupplierName { get; set; }
		public decimal ImportPrice { get; set; }
		public int Quantity { get; set; }
		public DateTime ManufactureDate { get; set; }
		public DateTime ExpirationDate { get; set; }
		public IFormFile Image { get; set; }
		public string? Note { get; set; }
	}
}
