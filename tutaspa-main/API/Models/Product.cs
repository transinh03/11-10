using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Product
    {
		[Key]
		public int ProductId { get; set; }

		[Required]
		[StringLength(200)]
		public string ProductName { get; set; }

		[Required]
		public int CategoryId { get; set; }
		public Category Category { get; set; }

		public string Description { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal CurrentSellingPrice { get; set; }

		[StringLength(300)]
		public string Images { get; set; }

		public ICollection<ProductBatch> ProductBatches { get; set; }

		[NotMapped]
		public int Quantity => ProductBatches?.Where(b => b.ExpiryDate > DateTime.Now).Sum(b => b.Quantity) ?? 0;
		public string? maProduct {  get; set; }
	}
}
