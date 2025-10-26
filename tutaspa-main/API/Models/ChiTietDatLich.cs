using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class ChiTietDatLich
    {
        [Key]
        public int ChiTietDatLichID { get; set; }

        [Required]
        [ForeignKey("DatLich")]
        public int DatLichID { get; set; }

        [Required]
        [ForeignKey("DichVu")]
        public int DichVuID { get; set; }

        public int soLuongDV { get; set; }

        // Điều hướng quan hệ
        [JsonIgnore]
        public DatLich? DatLich { get; set; }
        

        public DichVu? DichVu { get; set; }
    }
}
