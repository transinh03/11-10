using API.Models;

namespace API.DTOs
{
    public class DichVuDTO
    {
        public int Id { get; set; }
        public string TenDichVu { get; set; } 

        public string Mota { get; set; }

        public decimal Gia { get; set; }

        //public int SoLuong { get; set; }

        public int ThoiGian { get; set; } // phút

        public string HinhAnh { get; set; }


        public int LoaiDichVuID { get; set; }

        public LoaiDichVu LoaiDichVu { get; set; }
    }
}
