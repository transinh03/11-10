namespace API.DTOs.DatLich
{
    public class DatLichDTO
    {
        public string SoDienThoai { get; set; } = string.Empty;
        public DateTime ThoiGian { get; set; }
        public List<DichVuDatLichDTO>? DichVus { get; set; }
        public string? GhiChu { get; set; }
        public bool DatTruoc {  get; set; }
    }
    public class DichVuDatLichDTO
    {
        public int DichVuID { get; set; }
        public int SoLuong { get; set; }
    }
}
