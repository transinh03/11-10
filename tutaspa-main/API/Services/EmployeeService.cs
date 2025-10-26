using API.Data;
using API.DTOs.Employee;
using API.DTOs.Response;
using API.Extensions;
using API.IService;
using API.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Text;

namespace API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly int _pageSize = 5;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IValidator<CreateEmployeeDTO> _validator;
        private readonly IGmailService _mail;
        private readonly IMemoryCache _cache;
        private readonly ILogger<EmployeeService> _logger;

        // Cache configuration
        private static readonly TimeSpan _employeesCacheExpiration = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan _employeeDetailCacheExpiration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan _employeeRolesCacheExpiration = TimeSpan.FromMinutes(10);

        // Cache keys
        private const string EMPLOYEES_CACHE_KEY = "employees";
        private const string EMPLOYEE_ROLES_CACHE_KEY = "employee_roles";
        private const string EMPLOYEE_DETAIL_CACHE_KEY = "employee_detail";

        public EmployeeService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IValidator<CreateEmployeeDTO> validator,
            IGmailService mail,
            IMemoryCache cache,
            ILogger<EmployeeService> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _validator = validator;
            _mail = mail;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ServiceResponse> AddEmployeeAsync(CreateEmployeeDTO dto)
        {
            try
            {
                
                var validResult = await _validator.ValidateAsync(dto);
                if (!validResult.IsValid)
                    return Fail(string.Join(", ", validResult.Errors));

                
                var radPass = RandomPass();

                
                await _mail.SendEmailAsync(
                    dto.Email,
                    "Chào mừng đến với Tuta Spa",
                    $"<h1>Welcome {dto.Name}</h1><p>Tài khoản của bạn đã được tạo.</p><p>Mật khẩu của bạn là: <strong>{radPass}</strong></p>"
                );

                
                var user = new User
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    Name = dto.Name,
                    PhoneNumber = dto.PhoneNumber,
                    FisrtLogin = true,
                    Address = dto.Address,
                };

                
                var createResult = await _userManager.CreateAsync(user, radPass);
                if (!createResult.Succeeded)
                {
                    return Fail("Không thể tạo tài khoản nhân viên: " +
                                string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }

                
                if (!await _roleManager.RoleExistsAsync(dto.Role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole(dto.Role.ToString()));
                }
                await _userManager.AddToRoleAsync(user, dto.Role.ToString());

                
                await InvalidateEmployeeCaches();

                _logger.LogInformation("Employee created successfully with ID: {UserId}", user.Id);
                return Success("Thêm nhân viên thành công", new { userId = user.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee with email: {Email}", dto.Email);
                return Fail("Lỗi khi thêm nhân viên: " + ex.Message);
            }
        }


        public async Task<ServiceResponse> UpdateEmployeeAsync(UpdateEmployeeDTO dto)
        {
            var user = await _userManager.FindByIdAsync(dto.EmpId);
            user.CheckUserNull();

            try
            {
                user.Email = dto.Email;
                user.UserName = dto.Email;
                user.PhoneNumber = dto.PhoneNumber;
                user.Name = $"{dto.Name}";
                user.Address = dto.Address; 

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Invalidate specific employee cache and general cache
                    await InvalidateEmployeeCaches();
                    _cache.Remove($"{EMPLOYEE_DETAIL_CACHE_KEY}_{dto.EmpId}");

                    _logger.LogInformation("Employee updated successfully with ID: {EmployeeId}", dto.EmpId);
                    return Success("Cập nhật thông tin nhân viên thành công");
                }
                else
                {
                    return Fail("Cập nhật thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee with ID: {EmployeeId}", dto.EmpId);
                return Fail("Lỗi khi cập nhật nhân viên: " + ex.Message);
            }
        }

        public async Task<ServiceResponse> DeleteEmployeeAsync(string empId)
        {
            var user = await _userManager.FindByIdAsync(empId);
            user.CheckUserNull();

            try
            {
                user.IsDeleted = true;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await InvalidateEmployeeCaches();
                    _cache.Remove($"{EMPLOYEE_DETAIL_CACHE_KEY}_{empId}");

                    _logger.LogInformation("Employee deleted successfully with ID: {EmployeeId}", empId);
                    return Success("Xóa nhân viên thành công");
                }
                else
                {
                    return Fail("Xóa nhân viên thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee with ID: {EmployeeId}", empId);
                return Fail("Lỗi khi xóa nhân viên: " + ex.Message);
            }
        }

        public async Task<ServiceResponse> UpdateEmployeeRoleAsync(string empID, ERole role)
        {
            var user = await _userManager.FindByIdAsync(empID);
            user.CheckUserNull();

            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                var currentRole = roles.FirstOrDefault();

                if (currentRole != null && role.ToString() != currentRole)
                {
                    await _userManager.RemoveFromRoleAsync(user, currentRole);
                    await _userManager.AddToRoleAsync(user, role.ToString());
                }

                // Invalidate caches
                await InvalidateEmployeeCaches();
                _cache.Remove($"{EMPLOYEE_DETAIL_CACHE_KEY}_{empID}");
                _cache.Remove($"{EMPLOYEE_ROLES_CACHE_KEY}_{empID}");

                _logger.LogInformation("Employee role updated successfully for ID: {EmployeeId}", empID);
                return Success("Cập nhật vai trò nhân viên thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee role for ID: {EmployeeId}", empID);
                return Fail("Lỗi khi cập nhật vai trò nhân viên: " + ex.Message);
            }
        }

        public async Task<ServiceResponse> UpdateEmployeeStatus(string empId, bool status)
        {
            var user = await _userManager.FindByIdAsync(empId);
            user.CheckUserNull();

            try
            {
                user.IsDeleted = !status;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await InvalidateEmployeeCaches();
                    _cache.Remove($"{EMPLOYEE_DETAIL_CACHE_KEY}_{empId}");

                    _logger.LogInformation("Employee status updated successfully for ID: {EmployeeId}", empId);
                    return Success("Cập nhật trạng thái nhân viên thành công");
                }
                else
                {
                    return Fail("Cập nhật trạng thái thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee status for ID: {EmployeeId}", empId);
                return Fail("Lỗi khi cập nhật trạng thái nhân viên: " + ex.Message);
            }
        }

        public async Task<PagedResult<EmployeeDTO>> GetListEmployees(int page, string? keyword)
        {
            try
            {
                //var cacheKey = $"employees_list_page_{page}_keyword_{keyword ?? "empty"}";

                //// Try to get from cache first
                //var cachedResult = _cache.Get<PagedResult<EmployeeDTO>>(cacheKey);
                //if (cachedResult != null)
                //{
                //    _logger.LogDebug("Returning cached employee list for page {Page}, keyword: {Keyword}", page, keyword);
                //    return cachedResult;
                //}

                // Get employees with roles in one go to avoid N+1 problem
                var employeesWithRoles = await GetEmployeesWithRolesAsync();

                // Apply keyword filter if provided
                if (!string.IsNullOrEmpty(keyword))
                {
                    employeesWithRoles = employeesWithRoles
                        .Where(e => e.Employee.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Apply pagination
                var totalCount = employeesWithRoles.Count;
                var pagedEmployees = employeesWithRoles
                    .Skip((page - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                // Map to DTOs
                var employeeDTOs = pagedEmployees.Select(e => new EmployeeDTO
                {
                    Id = e.Employee.Id,
                    Email = e.Employee.Email,
                    Name = e.Employee.Name,
                    Phone = e.Employee.PhoneNumber,
                    Status = !e.Employee.IsDeleted,
                    Address = e.Employee.Address,
                    Role = e.Role
                }).OrderByDescending(x => x.Status ).ToList();

                var result = new PagedResult<EmployeeDTO>
                {
                    Data = employeeDTOs,
                    MaxPage = (int)Math.Ceiling((double)totalCount / _pageSize),
                    CurrentPage = page
                };

                // Cache the result for 5 minutes
                //_cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

                _logger.LogDebug("Employee list cached for page {Page}, keyword: {Keyword}", page, keyword);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee list for page {Page}, keyword: {Keyword}", page, keyword);
                throw;
            }
        }

        public async Task<ServiceResponse> GetEmployeeByIdAsync(string empId)
        {
            try
            {
                var cacheKey = $"{EMPLOYEE_DETAIL_CACHE_KEY}_{empId}";

                // Try cache first
                var cachedEmployee = _cache.Get<EmployeeDTO>(cacheKey);
                if (cachedEmployee != null)
                {
                    _logger.LogDebug("Returning cached employee detail for ID: {EmployeeId}", empId);
                    return Success("Lấy thông tin nhân viên thành công", cachedEmployee);
                }

                var user = await _userManager.FindByIdAsync(empId);
                if (user == null || user.IsDeleted)
                {
                    return Fail("Không tìm thấy nhân viên");
                }

                var roles = await GetEmployeeRolesAsync(empId);
                var employeeDto = new EmployeeDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Phone = user.PhoneNumber,
                    Status = !user.IsDeleted,
                    Role = roles.FirstOrDefault()
                };

                // Cache for 5 minutes
                _cache.Set(cacheKey, employeeDto, _employeeDetailCacheExpiration);

                _logger.LogDebug("Employee detail cached for ID: {EmployeeId}", empId);
                return Success("Lấy thông tin nhân viên thành công", employeeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee by ID: {EmployeeId}", empId);
                return Fail("Lỗi khi lấy thông tin nhân viên: " + ex.Message);
            }
        }

        private async Task<List<(User Employee, string Role)>> GetEmployeesWithRolesAsync()
        {
            var cacheKey = "employees_with_roles";
            var cached = _cache.Get<List<(User, string)>>(cacheKey);

            if (cached != null)
            {
                return cached;
            }

            var cashiers = await _userManager.GetUsersInRoleAsync(ERole.Cashier.ToString());
            var inventoryManagers = await _userManager.GetUsersInRoleAsync(ERole.InventoryManager.ToString());

            var result = new List<(User, string)>();
            result.AddRange(cashiers.Select(u => (u, ERole.Cashier.ToString())));
            result.AddRange(inventoryManagers.Select(u => (u, ERole.InventoryManager.ToString())));

            _cache.Set(cacheKey, result, _employeesCacheExpiration);
            return result;
        }

        private async Task<List<string>> GetEmployeeRolesAsync(string userId)
        {
            var cacheKey = $"{EMPLOYEE_ROLES_CACHE_KEY}_{userId}";
            var cached = _cache.Get<List<string>>(cacheKey);

            if (cached != null)
            {
                return cached;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            _cache.Set(cacheKey, roles, _employeeRolesCacheExpiration);

            return roles;
        }

        private async Task  InvalidateEmployeeCaches()
{
    var patterns = new[]
    {
        EMPLOYEES_CACHE_KEY,                // main employee cache
        "employees_with_roles",              // role + employee list
        "employees_list_",                   // paginated lists
        $"{EMPLOYEE_ROLES_CACHE_KEY}_",      // per-employee role cache
        $"{EMPLOYEE_DETAIL_CACHE_KEY}_"      // per-employee detail cache
    };

    // Access MemoryCache's internal dictionary
    var cacheField = typeof(MemoryCache).GetField("_entries",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

    if (cacheField?.GetValue(_cache) is IDictionary entries)
    {
        var keysToRemove = new List<object>();

        foreach (DictionaryEntry entry in entries)
        {
            string key = entry.Key.ToString();
            if (patterns.Any(p => key.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                keysToRemove.Add(entry.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
        }
    }
    else
    {
        // Fallback: remove the main keys if internal structure not found
        foreach (var pattern in patterns)
        {
            _cache.Remove(pattern);
        }
    }

    _logger.LogDebug("All employee-related caches invalidated");
}


        private string RandomPass()
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string special = "!@#";

            var rand = new Random();
            var chars = new StringBuilder();

            chars.Append(lower[rand.Next(lower.Length)]);
            chars.Append(upper[rand.Next(upper.Length)]);
            chars.Append(digits[rand.Next(digits.Length)]);
            chars.Append(special[rand.Next(special.Length)]);

            string allChars = lower + upper + digits + special;

            for (int i = 4; i < 9; i++)
            {
                chars.Append(allChars[rand.Next(allChars.Length)]);
            }

            return new string(chars.ToString().OrderBy(_ => rand.Next()).ToArray());
        }

        private ServiceResponse Fail(string message, object? data = null) =>
            new ServiceResponse(false, message, data);

        private ServiceResponse Success(string message, object? data = null) =>
            new ServiceResponse(true, message, data);
    }
}