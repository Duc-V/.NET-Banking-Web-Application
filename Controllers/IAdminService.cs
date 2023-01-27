using AdminAPI.Models;

public interface IAdminService
{
    Task<bool> Login(LoginModel loginModel);
}
