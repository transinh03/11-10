using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class ChiTietHoaDon
    {
        public int ChiTietHoaDonID { get; set; }
        [ForeignKey("HoaDon")]
        public int HoaDonID { get; set; }
        [ForeignKey("Product")]
        public int? SanPhamID { get; set; }
        [ForeignKey("DichVu")]
        public int? DichVuID { get; set; }
        public int SoLuongSP { get; set; }
        public decimal ThanhTien { get; set; }
        [JsonIgnore]
        public virtual HoaDon HoaDon { get; set; }
        
        public virtual Product? SanPham { get; set; }
        
        public virtual DichVu? DichVu { get; set; }
    }
}
