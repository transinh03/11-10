using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
	public class InventoryHistory
	{
		[Key]
		public int HistoryId { get; set; }

		public int ProductId { get; set; }
		public Product Product { get; set; }

		public string ActionType { get; set; } // Import | Export
		public int QuantityChanged { get; set; }

		public string? Note { get; set; }
		public DateTime Timestamp { get; set; }

		public DateTime? ExpirationDate { get; set; }
		public decimal? ImportPrice { get; set; }
		public string SupplierName { get; set; }

		public int? BatchId { get; set; } // ✅  theo dõi theo lô
		public ProductBatch Batch { get; set; }
	}
}
