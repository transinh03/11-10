using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
	public class ProductBatch
	{
		[Key]
		public int BatchId { get; set; }

		[Required]
		public int ProductId { get; set; }
		public Product Product { get; set; }
		[Required]
		[StringLength(50)]
		public string ProductCode { get; set; }

		[Required]
		[Column(TypeName = "decimal(18,2)")]
		public decimal ImportPrice { get; set; }

		[Required]
		public int Quantity { get; set; }

		[StringLength(200)]
		public string SupplierName { get; set; }

		[Required]
		public DateTime ManufactureDate { get; set; }

		[Required]
		public DateTime ExpiryDate { get; set; }
		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.Now;
	}
}
