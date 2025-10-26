using NuGet.Protocol;

namespace API.DTOs.Employee
{
    public class CreateEmployeeDTO
    {
        public string Email { get; set; }
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public ERole Role { get; set; }


    }
    public enum ERole
    {
        Cashier,
        InventoryManager
    }
}
