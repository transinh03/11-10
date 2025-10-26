namespace API.DTOs.Auth;
public interface IUserRepository
{
    Task<UserInfo?> GetUserByIdAsync(string userId);
}