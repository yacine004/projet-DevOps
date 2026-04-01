using csharp_web.Models;
using csharp_web.Data;
using Microsoft.EntityFrameworkCore;

namespace csharp_web.Repositories
{
    public interface ICommandeRepository
    {
        Task<List<Commande>> GetCommandesByClientIdAsync(int clientId);
        Task<Commande?> GetCommandeByIdAsync(int id);
        Task<Commande> CreateCommandeAsync(Commande commande);
        Task UpdateCommandeAsync(Commande commande);
        Task<List<LigneCommande>> GetLignesCommandeAsync(int commandeId);
        Task CreateLigneCommandeAsync(LigneCommande ligneCommande);
        Task<Paiement> CreatePaiementAsync(Paiement paiement);
        Task<Zone?> GetZoneByIdAsync(int id);
    }

    public class CommandeRepository : ICommandeRepository
    {
        private readonly ApplicationDbContext _context;

        public CommandeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Commande>> GetCommandesByClientIdAsync(int clientId)
        {
            return await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.Zone)
                .Include(c => c.Livreur)
                .Include(c => c.Paiement)
                .Where(c => c.ClientId == clientId)
                .OrderByDescending(c => c.Date)
                .ToListAsync();
        }

        public async Task<Commande?> GetCommandeByIdAsync(int id)
        {
            return await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.Zone)
                .Include(c => c.Livreur)
                .Include(c => c.Paiement)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Commande> CreateCommandeAsync(Commande commande)
        {
            _context.Commandes.Add(commande);
            await _context.SaveChangesAsync();
            return commande;
        }

        public async Task UpdateCommandeAsync(Commande commande)
        {
            _context.Commandes.Update(commande);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LigneCommande>> GetLignesCommandeAsync(int commandeId)
        {
            return await _context.LigneCommandes
                .Include(lc => lc.Burger)
                .Include(lc => lc.Menu)
                .Include(lc => lc.Complement)
                .Where(lc => lc.CommandeId == commandeId)
                .ToListAsync();
        }

        public async Task CreateLigneCommandeAsync(LigneCommande ligneCommande)
        {
            _context.LigneCommandes.Add(ligneCommande);
            await _context.SaveChangesAsync();
        }

        public async Task<Paiement> CreatePaiementAsync(Paiement paiement)
        {
            _context.Paiements.Add(paiement);
            await _context.SaveChangesAsync();
            return paiement;
        }

        public async Task<Zone?> GetZoneByIdAsync(int id)
        {
            return await _context.Zones.FirstOrDefaultAsync(z => z.Id == id);
        }
    }
}