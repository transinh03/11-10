using System.ComponentModel.DataAnnotations.Schema;
using API.Models;
using Newtonsoft.Json;

public class DanhGia
{
    public int Id { get; set; }

    [ForeignKey("DichVu")] // 🔥 dòng này rất quan trọng
    public int MaDichVu { get; set; }
    [JsonIgnore]
    public DichVu? DichVu { get; set; }

    //public int? MaNhanVien { get; set; }
    //public Employee? NhanVien { get; set; }

    public bool IsActive { get; set; } = true;

    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    [JsonIgnore]
    public User? User { get; set; }

    public int SoSao { get; set; } = 5;
    public string? NoiDung { get; set; }

    public bool AnDanh { get; set; } = false;
    public bool DaDuyet { get; set; } = false;

    public DateTime NgayTao { get; set; } = DateTime.Now;
}
