using csharp_web.Models;
using csharp_web.Services;
using Microsoft.AspNetCore.Mvc;

namespace csharp_web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IClientService _clientService;

        public AccountController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string nom, string telephone)
        {
            if (string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(telephone))
            {
                ViewBag.Error = "Veuillez saisir votre nom et numéro de téléphone.";
                return View();
            }

            var client = await _clientService.AuthenticateAsync(nom, telephone);
            if (client != null)
            {
                // Stocker en session
                HttpContext.Session.SetInt32("ClientId", client.Id);
                HttpContext.Session.SetString("ClientNom", client.Nom);
                HttpContext.Session.SetString("ClientPrenom", client.Prenom);
                HttpContext.Session.SetString("ClientTelephone", client.Telephone);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Client non trouvé. Veuillez créer un compte.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string nom, string prenom, string telephone)
        {
            if (string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(prenom) || string.IsNullOrEmpty(telephone))
            {
                ViewBag.Error = "Tous les champs sont obligatoires.";
                return View();
            }

            if (await _clientService.IsRegisteredAsync(nom, telephone))
            {
                ViewBag.Error = "Un client avec ce nom et téléphone existe déjà.";
                return View();
            }

            await _clientService.RegisterAsync(nom, prenom, telephone);
            TempData["SuccessMessage"] = "Compte créé avec succès. Vous pouvez maintenant vous connecter.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Index()
        {
            var clientId = HttpContext.Session.GetInt32("ClientId");
            if (!clientId.HasValue)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
    }
}