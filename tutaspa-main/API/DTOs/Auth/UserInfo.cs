namespace API.DTOs.Auth;

public class UserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    
    public string Role { get; set; } 

}