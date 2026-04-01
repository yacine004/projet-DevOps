using csharp_web.Data;
using csharp_web.Models;
using csharp_web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace csharp_web.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Client?> GetByNomAndTelephoneAsync(string nom, string telephone)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.Nom == nom && c.Telephone == telephone);
        }

        public async Task AddAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string nom, string telephone)
        {
            return await _context.Clients
                .AnyAsync(c => c.Nom == nom && c.Telephone == telephone);
        }
    }
}