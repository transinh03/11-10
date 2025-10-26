using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Voucher
    {
        [Key]
        public int VoucherID { get; set; }

        [StringLength(50)]
        public string MaCode { get; set; } = string.Empty;

        public float GiaTriGiam { get; set; }

        public byte KieuGiamGia { get; set; }  // 0 = %, 1 = số tiền

        public DateTime NgayBatDau { get; set; }

        public DateTime NgayKetThuc { get; set; }

        public int SoLuong { get; set; }

    }
}
