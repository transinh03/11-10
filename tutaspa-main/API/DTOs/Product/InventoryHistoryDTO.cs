namespace API.DTOs.Product
{
	public class InventoryHistoryDTO
	{
		public int ProductId { get; set; }
		public string ProductCode { get; set; }
		public int BatchId { get; set; }
		public string ProductName { get; set; }
		public string ActionType { get; set; } // Import / Export
		public int QuantityChanged { get; set; }
		public DateTime Timestamp { get; set; }
		public string SupplierName { get; set; }
		public decimal? ImportPrice { get; set; }
		public DateTime? ManufactureDate { get; set; }
		public DateTime? ExpirationDate { get; set; }
		public string? Note { get; set; }
		
	}
}
