using csharp_web.Models;
using csharp_web.Repositories;
using csharp_web.Services;

namespace csharp_web.Services
{
    public interface ICommandeService
    {
        Task<Commande> CreerCommandeAsync(int clientId, List<PanierItem> panierItems, TypeCommande typeCommande, string? modeConsommation, int? zoneId = null);
        Task<List<Commande>> GetCommandesClientAsync(int clientId);
        Task<Commande?> GetCommandeByIdAsync(int id);
        Task AnnulerCommandeAsync(int commandeId, int clientId);
        Task<List<LigneCommande>> GetLignesCommandeAsync(int commandeId);
        double CalculerTotalCommande(List<LigneCommande> lignes);
        Task MarquerCommandePayeeAsync(int commandeId, MethodePaiement methodePaiement);
    }

    public class CommandeService : ICommandeService
    {
        private readonly ICommandeRepository _commandeRepository;
        private readonly IClientService _clientService;

        public CommandeService(ICommandeRepository commandeRepository, IClientService clientService)
        {
            _commandeRepository = commandeRepository;
            _clientService = clientService;
        }

        public async Task<Commande> CreerCommandeAsync(int clientId, List<PanierItem> panierItems, TypeCommande typeCommande, string? modeConsommation, int? zoneId = null)
        {
            // Calculer le total des articles
            var total = panierItems.Sum(item => item.Prix * item.Quantite);

            // Ajouter le prix de livraison si c'est une livraison
            if (typeCommande == TypeCommande.Livraison && zoneId.HasValue)
            {
                var zone = await _commandeRepository.GetZoneByIdAsync(zoneId.Value);
                if (zone != null)
                {
                    total += zone.Prix;
                }
            }

            // Créer la commande (sans paiement pour l'instant)
            var commande = new Commande
            {
                ClientId = clientId,
                Etat = EtatCommande.EnCours,
                Date = DateTime.UtcNow,
                Type = typeCommande,
                Total = total,
                ZoneId = zoneId // Ajouter la zone si livraison
            };

            var commandeCreee = await _commandeRepository.CreateCommandeAsync(commande);

            // Créer les lignes de commande
            foreach (var item in panierItems)
            {
                var ligneCommande = new LigneCommande
                {
                    CommandeId = commandeCreee.Id,
                    Quantite = item.Quantite
                };

                // Assigner l'ID selon le type de produit
                switch (item.Type)
                {
                    case TypeProduit.Burger:
                        ligneCommande.BurgerId = item.Id;
                        break;
                    case TypeProduit.Menu:
                        ligneCommande.MenuId = item.Id;
                        break;
                    case TypeProduit.Complement:
                        ligneCommande.ComplementId = item.Id;
                        break;
                }

                await _commandeRepository.CreateLigneCommandeAsync(ligneCommande);
            }

            return commandeCreee;
        }

        public async Task<List<Commande>> GetCommandesClientAsync(int clientId)
        {
            var commandes = await _commandeRepository.GetCommandesByClientIdAsync(clientId);
            foreach (var commande in commandes)
            {
                var lignes = await GetLignesCommandeAsync(commande.Id);
                commande.Total = CalculerTotalCommande(lignes);

                // Ajouter le prix de livraison si c'est une livraison
                if (commande.Type == TypeCommande.Livraison && commande.Zone != null)
                {
                    commande.Total += commande.Zone.Prix;
                }
            }
            return commandes;
        }

        public async Task<Commande?> GetCommandeByIdAsync(int id)
        {
            var commande = await _commandeRepository.GetCommandeByIdAsync(id);
            if (commande != null)
            {
                var lignes = await GetLignesCommandeAsync(id);
                commande.Total = CalculerTotalCommande(lignes);

                // Ajouter le prix de livraison si c'est une livraison
                if (commande.Type == TypeCommande.Livraison && commande.Zone != null)
                {
                    commande.Total += commande.Zone.Prix;
                }
            }
            return commande;
        }

        public async Task AnnulerCommandeAsync(int commandeId, int clientId)
        {
            var commande = await _commandeRepository.GetCommandeByIdAsync(commandeId);

            if (commande == null || commande.ClientId != clientId)
            {
                throw new Exception("Commande non trouvée ou accès non autorisé");
            }

            if (commande.Etat != EtatCommande.EnCours)
            {
                throw new Exception("Seules les commandes en cours peuvent être annulées");
            }

            commande.Etat = EtatCommande.Annulee;
            await _commandeRepository.UpdateCommandeAsync(commande);
        }

        public async Task<List<LigneCommande>> GetLignesCommandeAsync(int commandeId)
        {
            return await _commandeRepository.GetLignesCommandeAsync(commandeId);
        }

        public async Task MarquerCommandePayeeAsync(int commandeId, MethodePaiement methodePaiement)
        {
            var commande = await _commandeRepository.GetCommandeByIdAsync(commandeId);
            if (commande == null)
            {
                throw new Exception("Commande introuvable");
            }

            // Créer le paiement
            var paiement = new Paiement
            {
                CommandeId = commandeId,
                Montant = commande.Total,
                Methode = methodePaiement,
                DatePaiement = DateTime.UtcNow
            };

            await _commandeRepository.CreatePaiementAsync(paiement);

            // Changer l'état de la commande à Terminee
            commande.Etat = EtatCommande.Terminee;
            commande.Paiement = paiement;

            await _commandeRepository.UpdateCommandeAsync(commande);
        }

        public double CalculerTotalCommande(List<LigneCommande> lignes)
        {
            return lignes.Sum(l => l.PrixUnitaire * l.Quantite);
        }
    }
}