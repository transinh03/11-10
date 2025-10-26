namespace API.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime NgayVaoLam { get; set; }
        public string ChucVu { get; set; } = string.Empty;

        // Liên kết đánh giá
        public ICollection<DanhGia>? DanhGias { get; set; }
    }
}
