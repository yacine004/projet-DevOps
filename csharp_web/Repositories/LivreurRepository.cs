using csharp_web.Data;
using csharp_web.Models;
using Microsoft.EntityFrameworkCore;

namespace csharp_web.Repositories
{
    public class LivreurRepository : ILivreurRepository
    {
        private readonly ApplicationDbContext _context;

        public LivreurRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Livreur?> AuthenticateAsync(string telephone)
        {
            return await _context.Livreurs
                .Include(l => l.Zone)
                .FirstOrDefaultAsync(l => l.Telephone == telephone);
        }

        public async Task<List<Commande>> GetLivraisonsDuJourAsync(int livreurId)
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.Zone)
                .Where(c => c.LivreurId == livreurId &&
                           (c.Etat == EtatCommande.Validee || c.Etat == EtatCommande.EnCours) &&
                           c.Type == TypeCommande.Livraison &&
                           c.Date.Date == today)
                .ToListAsync();
        }

        public async Task MarquerLivraisonTermineeAsync(int commandeId)
        {
            var commande = await _context.Commandes.FindAsync(commandeId);
            if (commande != null)
            {
                commande.Etat = EtatCommande.Terminee;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateLivreurAsync(Livreur livreur)
        {
            _context.Livreurs.Add(livreur);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Livreur>> GetAllLivreursAsync()
        {
            return await _context.Livreurs.Include(l => l.Zone).ToListAsync();
        }

        public async Task<Livreur?> GetLivreurByIdAsync(int id)
        {
            return await _context.Livreurs.Include(l => l.Zone).FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task UpdateLivreurAsync(Livreur livreur)
        {
            _context.Livreurs.Update(livreur);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLivreurAsync(int id)
        {
            var livreur = await _context.Livreurs.FindAsync(id);
            if (livreur != null)
            {
                _context.Livreurs.Remove(livreur);
                await _context.SaveChangesAsync();
            }
        }
    }
}