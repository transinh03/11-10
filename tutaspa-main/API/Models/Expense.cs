using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
	public class Expense
	{
		[Key]
		public int ExpenseId { get; set; }

		[Required]
		[Column(TypeName = "decimal(18,2)")]
		public decimal Amount { get; set; }

		[Required]
		[StringLength(50)]
		public string ExpenseType { get; set; }

		[Required]
		public DateTime Date { get; set; }

		[StringLength(200)]
		public string Note { get; set; }
	}
}
