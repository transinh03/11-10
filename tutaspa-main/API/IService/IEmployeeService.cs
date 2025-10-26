using API.DTOs.Employee;
using API.DTOs.Response;

namespace API.IService
{
    public interface IEmployeeService
    {
        public Task<ServiceResponse> AddEmployeeAsync(CreateEmployeeDTO dto);
        public Task<ServiceResponse> UpdateEmployeeAsync(UpdateEmployeeDTO dto);

        public Task<ServiceResponse> UpdateEmployeeStatus(string empId, bool status);

        public Task<ServiceResponse> DeleteEmployeeAsync(string empId);

        public Task<ServiceResponse> UpdateEmployeeRoleAsync(string empID, ERole role);


        public Task<ServiceResponse> GetEmployeeByIdAsync(string empId);


        public Task<PagedResult<EmployeeDTO>> GetListEmployees(int page ,string keyword);

        //public Task<PagedResult<EmployeeDTO>> Search(string keyword, int page = 1);
        
        }
    }
