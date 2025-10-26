namespace API.DTOs.ThongKe
{
	public class ThongKeThuChiDTO
	{
		public List<ThongKeThuDTO> Thu { get; set; }
		public List<ThongKeChiDTO> Chi { get; set; }
		public decimal TongThu { get; set; }
		public decimal TongChi { get; set; }
	}
	public class ThongKeThuDTO
	{
		public string LoaiThu { get; set; }
		public decimal SoTien { get; set; }
		public float PhanTram { get; set; }
	}

	public class ThongKeChiDTO
	{
		public string LoaiChi { get; set; }
		public decimal SoTien { get; set; }
		public float PhanTram { get; set; }
	}
	public class ExpenseDTO
	{
		public decimal Amount { get; set; }
		public string ExpenseType { get; set; }
		public DateTime Date { get; set; }
		public string Note { get; set; }
	}
}
