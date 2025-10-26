using Net.payOS.Types;

namespace API.DTOs.HoaDon
{
    public class PaymentDTO
    {
        public int TotalAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<ItemPaymentDTO> Items { get; set; } = new List<ItemPaymentDTO>();
        public string cancelUrl { get; set; } = string.Empty;
        public string returnUrl { get; set; } = string.Empty;
    }
}
