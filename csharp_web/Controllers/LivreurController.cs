using Microsoft.AspNetCore.Mvc;
using csharp_web.Models;
using csharp_web.Services;

namespace csharp_web.Controllers
{
    public class LivreurController : Controller
    {
        private readonly ILivreurService _livreurService;

        public LivreurController(ILivreurService livreurService)
        {
            _livreurService = livreurService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string telephone)
        {
            if (string.IsNullOrEmpty(telephone))
            {
                ViewBag.Error = "Veuillez saisir votre numéro de téléphone.";
                return View();
            }

            var livreur = await _livreurService.AuthenticateAsync(telephone);
            if (livreur != null)
            {
                // Stocker en session
                HttpContext.Session.SetInt32("LivreurId", livreur.Id);
                HttpContext.Session.SetString("LivreurNom", livreur.Nom);
                HttpContext.Session.SetString("LivreurPrenom", livreur.Prenom);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = "Livreur non trouvé.";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var livreurId = HttpContext.Session.GetInt32("LivreurId");
            if (!livreurId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var livraisons = await _livreurService.GetLivraisonsDuJourAsync(livreurId.Value);
            ViewBag.Livraisons = livraisons;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MarquerTerminee(int commandeId)
        {
            var livreurId = HttpContext.Session.GetInt32("LivreurId");
            if (!livreurId.HasValue)
            {
                return Json(new { success = false, message = "Non connecté" });
            }

            try
            {
                await _livreurService.MarquerLivraisonTermineeAsync(commandeId, livreurId.Value);
                return Json(new { success = true, message = "Livraison marquée comme terminée" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("LivreurId");
            HttpContext.Session.Remove("LivreurNom");
            HttpContext.Session.Remove("LivreurPrenom");
            return RedirectToAction("Login");
        }
    }
}