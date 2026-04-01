using csharp_web.Models;

namespace csharp_web.Repositories
{
    public interface IClientRepository
    {
        Task<Client?> GetByNomAndTelephoneAsync(string nom, string telephone);
        Task AddAsync(Client client);
        Task<bool> ExistsAsync(string nom, string telephone);
    }
}