using API.DTOs.Employee;
using API.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost]
        public async Task< IActionResult> CreateAsync([FromBody] CreateEmployeeDTO request )
        {
            var result = await _employeeService.AddEmployeeAsync(request);

            if (result.Success)
            {
                return(Ok(result));
            }

            return BadRequest(new {message = result.Message});
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateEmployeeDTO request)
        {
            var result = await _employeeService.UpdateEmployeeAsync(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpPatch("status/{empId}")]
        public async Task<IActionResult> UpdateStatusAsync(string empId, [FromQuery] bool status)
        {
            var result = await _employeeService.UpdateEmployeeStatus(empId, status);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpDelete("{empId}")]
        public async Task<IActionResult> DeleteAsync(string empId)
        {
            var result = await _employeeService.DeleteEmployeeAsync(empId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpPatch("role/{empId}")]
        public async Task<IActionResult> UpdateRoleAsync(string empId, [FromQuery] ERole role)
        {
            var result = await _employeeService.UpdateEmployeeRoleAsync(empId, role);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetListEmployees([FromQuery] string? keyword  ,[FromQuery]int page =1 )
        {
            if(page < 1)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            var empoyees = await _employeeService.GetListEmployees(page,keyword) ;


            return Ok(empoyees);
        }


    }
}
