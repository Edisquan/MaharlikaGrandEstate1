using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MaharlikaGrandEstate.Models;

namespace MaharlikaGrandEstate.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context; // Add your DbContext

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var featuredProperties = _context.Properties
            .Where(p => p.IsApproved) // Fetch only approved properties
            .Take(3) // Display the top 3
            .ToList();

        return View(featuredProperties); // Pass the list to the view
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
