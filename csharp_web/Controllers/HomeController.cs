using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using csharp_web.Models;
using csharp_web.Data;
using Microsoft.EntityFrameworkCore;
using csharp_web.Services;

namespace csharp_web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IPanierService _panierService;

    public HomeController(ApplicationDbContext context, IPanierService panierService)
    {
        _context = context;
        _panierService = panierService;
    }

    public async Task<IActionResult> Index(string filter = "all", string search = "")
    {
        ViewBag.Filter = filter;
        ViewBag.Search = search;
        ViewBag.CartItemCount = _panierService.GetCartItemCount();

        IQueryable<Burger> burgersQuery = _context.Burgers;
        IQueryable<Complement> complementsQuery = _context.Complements;
        IQueryable<Menu> menusQuery = _context.Menus;

        if (!string.IsNullOrEmpty(search))
        {
            burgersQuery = burgersQuery.Where(b => b.Nom.Contains(search) || b.Description.Contains(search));
            complementsQuery = complementsQuery.Where(c => c.Nom.Contains(search) || c.Description.Contains(search));
            menusQuery = menusQuery.Where(m => m.Nom.Contains(search) || m.Description.Contains(search));
        }

        if (filter == "all" || filter == "burgers")
        {
            ViewBag.Burgers = await burgersQuery.ToListAsync();
        }

        if (filter == "all" || filter == "complements")
        {
            ViewBag.Complements = await complementsQuery.ToListAsync();
        }

        if (filter == "all" || filter == "menus")
        {
            ViewBag.Menus = await menusQuery.ToListAsync();
        }

        return View();
    }

    [HttpPost]
    public IActionResult AddToCart(TypeProduit type, int id, string nom, double prix, string image)
    {
        _panierService.AddToCart(type, id, nom, prix, image);
        return Json(new { success = true, itemCount = _panierService.GetCartItemCount() });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
