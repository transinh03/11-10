namespace API.DTOs.Product
{
    public class ProductDTO
    {
        public int SanPhamId { get; set; }
        public string TenSP { get; set; }
        public int SoLuong { get; set; }
        public string MoTa { get; set; }
        public decimal Gia { get; set; }
        public string HinhAnh { get; set; }
        public int LoaiSanPhamId { get; set; }

    }
    public class ProductCreateDTO
    {
		public string ProductName { get; set; }
		public string Description { get; set; }
		public int CategoryId { get; set; }
		public IFormFile Image { get; set; }

		// Lô hàng đầu tiên
		public decimal ImportPrice { get; set; }
		public decimal SellingPrice { get; set; }
		public int Quantity { get; set; }
		public string SupplierName { get; set; }
		public DateTime ManufactureDate { get; set; }
		public DateTime ExpiryDate { get; set; }
        public string? maProduct { get; set; }
    }
	public class ProductUpdateDTO
	{
		public string ProductName { get; set; }
		public string Description { get; set; }
		public decimal SellingPrice { get; set; }
		public int CategoryId { get; set; }
		public IFormFile? Image { get; set; } // Ảnh có thể bỏ qua nếu không cập nhật
	}

}
