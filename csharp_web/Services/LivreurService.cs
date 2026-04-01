using csharp_web.Models;
using csharp_web.Repositories;

namespace csharp_web.Services
{
    public class LivreurService : ILivreurService
    {
        private readonly ILivreurRepository _livreurRepository;

        public LivreurService(ILivreurRepository livreurRepository)
        {
            _livreurRepository = livreurRepository;
        }

        public async Task<Livreur?> AuthenticateAsync(string telephone)
        {
            return await _livreurRepository.AuthenticateAsync(telephone);
        }

        public async Task<List<Commande>> GetLivraisonsDuJourAsync(int livreurId)
        {
            return await _livreurRepository.GetLivraisonsDuJourAsync(livreurId);
        }

        public async Task MarquerLivraisonTermineeAsync(int commandeId, int livreurId)
        {
            var livraisons = await _livreurRepository.GetLivraisonsDuJourAsync(livreurId);
            if (livraisons.Any(c => c.Id == commandeId))
            {
                await _livreurRepository.MarquerLivraisonTermineeAsync(commandeId);
            }
            else
            {
                throw new Exception("Commande non trouvée ou accès non autorisé");
            }
        }

        public async Task CreateLivreurAsync(Livreur livreur)
        {
            await _livreurRepository.CreateLivreurAsync(livreur);
        }

        public async Task<List<Livreur>> GetAllLivreursAsync()
        {
            return await _livreurRepository.GetAllLivreursAsync();
        }

        public async Task<Livreur?> GetLivreurByIdAsync(int id)
        {
            return await _livreurRepository.GetLivreurByIdAsync(id);
        }

        public async Task UpdateLivreurAsync(Livreur livreur)
        {
            await _livreurRepository.UpdateLivreurAsync(livreur);
        }

        public async Task DeleteLivreurAsync(int id)
        {
            await _livreurRepository.DeleteLivreurAsync(id);
        }
    }
}