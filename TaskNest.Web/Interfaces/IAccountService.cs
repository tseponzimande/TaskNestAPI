namespace TaskNest.Web.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto model);
        Task<string?> LoginAsync(LoginDto model);

    }
}
