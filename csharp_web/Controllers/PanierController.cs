using csharp_web.Models;
using csharp_web.Services;
using Microsoft.AspNetCore.Mvc;
using csharp_web.Data;
using Microsoft.EntityFrameworkCore;

namespace csharp_web.Controllers
{
    public class PanierController : Controller
    {
        private readonly IPanierService _panierService;
        private readonly ICommandeService _commandeService;
        private readonly ApplicationDbContext _context;

        public PanierController(IPanierService panierService, ICommandeService commandeService, ApplicationDbContext context)
        {
            _panierService = panierService;
            _commandeService = commandeService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var items = _panierService.GetCartItems();
            var itemsTotal = _panierService.GetTotal();
            var total = itemsTotal;
            var mode = _panierService.GetModeConsommation();
            double deliveryPrice = 0;

            if (mode == "Livraison")
            {
                var zoneId = _panierService.GetZoneLivraison();
                if (zoneId.HasValue)
                {
                    var zone = await _context.Zones.FindAsync(zoneId.Value);
                    if (zone != null)
                    {
                        deliveryPrice = zone.Prix;
                        total += zone.Prix;
                    }
                }
            }

            ViewBag.Total = total;
            ViewBag.ItemsTotal = itemsTotal;
            ViewBag.DeliveryPrice = deliveryPrice;
            ViewBag.ModeConsommation = mode;
            ViewBag.MethodePaiement = _panierService.GetMethodePaiement();
            ViewBag.Zones = await _context.Zones.AsNoTracking().ToListAsync();
            ViewBag.SelectedZoneId = _panierService.GetZoneLivraison();
            return View(items);
        }

        [HttpPost]
        public IActionResult AddToCart(TypeProduit type, int id, string nom, double prix, string image)
        {
            _panierService.AddToCart(type, id, nom, prix, image);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int itemId, int quantity)
        {
            _panierService.UpdateQuantity(itemId, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int itemId)
        {
            _panierService.RemoveFromCart(itemId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SetModeConsommation(string mode)
        {
            _panierService.SetModeConsommation(mode);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SetMethodePaiement(MethodePaiement methode)
        {
            _panierService.SetMethodePaiement(methode);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SetZoneLivraison(int zoneId)
        {
            _panierService.SetZoneLivraison(zoneId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PasserCommande()
        {
            var clientId = HttpContext.Session.GetInt32("ClientId");

            if (!clientId.HasValue)
            {
                TempData["ErrorMessage"] = "Vous devez être connecté pour passer une commande.";
                return RedirectToAction("Login", "Account");
            }

            var panierItems = _panierService.GetCartItems();
            Console.WriteLine($"PasserCommande - Nombre d'articles dans le panier: {panierItems.Count}");

            if (!panierItems.Any())
            {
                Console.WriteLine("PasserCommande - Panier vide, redirection vers Index");
                return RedirectToAction("Index");
            }

            // Convertir le mode de consommation en TypeCommande
            var modeConsommation = _panierService.GetModeConsommation();
            var typeCommande = modeConsommation switch
            {
                "Sur place" => TypeCommande.SurPlace,
                "À emporter" => TypeCommande.AEmporter,
                "Livraison" => TypeCommande.Livraison,
                _ => TypeCommande.SurPlace
            };

            var methodePaiement = _panierService.GetMethodePaiement();
            Console.WriteLine($"PasserCommande - Mode: {modeConsommation}, Type: {typeCommande}");

            try
            {
                Console.WriteLine("PasserCommande - Création de la commande...");
                int? zoneId = null;
                if (typeCommande == TypeCommande.Livraison)
                {
                    zoneId = _panierService.GetZoneLivraison();
                    if (!zoneId.HasValue)
                    {
                        TempData["ErrorMessage"] = "Veuillez sélectionner une zone de livraison.";
                        return RedirectToAction("Index");
                    }
                }
                await _commandeService.CreerCommandeAsync(clientId.Value, panierItems, typeCommande, modeConsommation, zoneId);
                _panierService.ClearCart();
                TempData["SuccessMessage"] = "Votre commande a été passée avec succès !";
                Console.WriteLine("PasserCommande - Commande créée avec succès, redirection vers Commande/Index");
                return RedirectToAction("Index", "Commande");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PasserCommande - Erreur: {ex.Message}");
                TempData["ErrorMessage"] = "Erreur lors de la création de la commande : " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}