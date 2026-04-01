using Microsoft.AspNetCore.Mvc;
using csharp_web.Models;
using csharp_web.Services;
using Microsoft.AspNetCore.Http;

namespace csharp_web.Controllers
{
    public class CommandeController : Controller
    {
        private readonly ICommandeService _commandeService;
        private readonly IPanierService _panierService;
        private readonly IClientService _clientService;

        public CommandeController(ICommandeService commandeService, IPanierService panierService, IClientService clientService)
        {
            _commandeService = commandeService;
            _panierService = panierService;
            _clientService = clientService;
        }

        public async Task<IActionResult> Index()
        {
            // Vérifier si l'utilisateur est connecté
            var clientId = HttpContext.Session.GetInt32("ClientId");
            if (!clientId.HasValue)
            {
                TempData["ErrorMessage"] = "Vous devez être connecté pour voir vos commandes.";
                return RedirectToAction("Login", "Account");
            }

            var commandes = await _commandeService.GetCommandesClientAsync(clientId.Value);

            // Pour chaque commande, récupérer les lignes et calculer le total
            var commandesAvecDetails = new List<dynamic>();
            foreach (var commande in commandes)
            {
                var lignes = await _commandeService.GetLignesCommandeAsync(commande.Id);
                var total = _commandeService.CalculerTotalCommande(lignes);

                // Ajouter le prix de livraison si c'est une livraison
                if (commande.Type == TypeCommande.Livraison && commande.Zone != null)
                {
                    total += commande.Zone.Prix;
                }

                commandesAvecDetails.Add(new
                {
                    Commande = commande,
                    Lignes = lignes,
                    Total = total
                });
            }

            ViewBag.CommandesAvecDetails = commandesAvecDetails;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AnnulerCommande(int commandeId)
        {
            var clientId = HttpContext.Session.GetInt32("ClientId");
            if (!clientId.HasValue)
            {
                return Json(new { success = false, message = "Vous devez être connecté pour annuler une commande." });
            }

            try
            {
                await _commandeService.AnnulerCommandeAsync(commandeId, clientId.Value);
                TempData["SuccessMessage"] = "Commande annulée avec succès";
                return Json(new { success = true, message = "Commande annulée avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PayerCommande(int commandeId, MethodePaiement methodePaiement)
        {
            var clientId = HttpContext.Session.GetInt32("ClientId");
            if (!clientId.HasValue)
            {
                return Json(new { success = false, message = "Vous devez être connecté pour payer une commande." });
            }

            try
            {
                // Récupérer la commande
                var commande = await _commandeService.GetCommandeByIdAsync(commandeId);
                if (commande == null || commande.ClientId != clientId.Value)
                {
                    return Json(new { success = false, message = "Commande introuvable" });
                }

                if (commande.Etat != EtatCommande.Validee)
                {
                    return Json(new { success = false, message = "La commande doit être validée pour être payée" });
                }

                // Simuler le paiement (en production, intégrer Wave/Orange Money API)
                // Ici, on marque simplement comme payé et change l'état à Terminee

                await _commandeService.MarquerCommandePayeeAsync(commandeId, methodePaiement);
                return Json(new { success = true, message = "Paiement effectué avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}