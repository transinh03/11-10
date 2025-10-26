namespace API.DTOs.Product
{
	public class ProductBatchCreateDTO
	{
		public int ProductId { get; set; }
		public decimal ImportPrice { get; set; }
		public int Quantity { get; set; }
		public string SupplierName { get; set; }
		public DateTime ManufactureDate { get; set; }
		public DateTime ExpiryDate { get; set; }
	}
	public class ProductBatchDTO
	{
		public int BatchId { get; set; }
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public string ProductCode { get; set; }
		public decimal ImportPrice { get; set; }
		public int Quantity { get; set; }
		public string SupplierName { get; set; }
		public DateTime ManufactureDate { get; set; }
		public DateTime ExpiryDate { get; set; }
	}
}
