using csharp_web.Models;

namespace csharp_web.Services
{
    public interface IClientService
    {
        Task<Client?> AuthenticateAsync(string nom, string telephone);
        Task RegisterAsync(string nom, string prenom, string telephone);
        Task<bool> IsRegisteredAsync(string nom, string telephone);
    }
}