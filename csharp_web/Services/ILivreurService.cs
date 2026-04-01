using csharp_web.Models;

namespace csharp_web.Services
{
    public interface ILivreurService
    {
        Task<Livreur?> AuthenticateAsync(string telephone);
        Task<List<Commande>> GetLivraisonsDuJourAsync(int livreurId);
        Task MarquerLivraisonTermineeAsync(int commandeId, int livreurId);
        Task CreateLivreurAsync(Livreur livreur);
        Task<List<Livreur>> GetAllLivreursAsync();
        Task<Livreur?> GetLivreurByIdAsync(int id);
        Task UpdateLivreurAsync(Livreur livreur);
        Task DeleteLivreurAsync(int id);
    }
}