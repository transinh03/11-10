using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.DTOs.Auth;
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserInfo?> GetUserByIdAsync(string userId)
    {
        return await _context.Users.Select(u => new UserInfo
        {
            Id = u.Id,
            Name = u.Name,
            PhoneNumber = u.PhoneNumber
        })
        .FirstOrDefaultAsync(u => u.Id == userId);
    }

}