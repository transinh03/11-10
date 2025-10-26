namespace API.DTOs.HoaDon
{
    public class HoaDonDTO
    {
        public DateTime NgayTao { get; set; }
        public string? MaGiamGia { get; set; }
        public string HinhThucThanhToan { get; set; }
        public byte TrangThai { get; set; }
        public decimal? TienKhachDua { get; set; }
        public decimal? TienThoiLai { get; set; }
        public decimal? tienGiam {  get; set; }
        public string NhanVienID { get; set; }
        public string UserID { get; set; }

        public List<ChiTietHoaDonDTO> ChiTietHoaDon { get; set; }
    }
}
