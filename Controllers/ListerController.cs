using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MaharlikaGrandEstate.Data;
using MaharlikaGrandEstate.Models;
using Microsoft.AspNetCore.Identity;

namespace MaharlikaGrandEstate.Controllers
{
    [Authorize(Roles = "Lister")]
    public class ListerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ListerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Lister/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Count properties created by the lister
            var myPropertiesCount = await _context.Properties
                .Where(p => p.ListerId == userId)
                .CountAsync();

            // Count pending properties for the lister
            var pendingCount = await _context.Properties
                .Where(p => p.ListerId == userId && !p.IsApproved)
                .CountAsync();

            // Count inquiries for properties owned by the lister
            var inquiriesCount = await _context.Inquiries
                .Where(i => _context.Properties.Any(p => p.Id == i.PropertyId && p.ListerId == userId))
                .CountAsync();

            ViewBag.MyPropertiesCount = myPropertiesCount;
            ViewBag.PendingCount = pendingCount;
            ViewBag.InquiriesCount = inquiriesCount;

            return View();
        }

        // NEW: GET: Lister/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            // Optionally, restrict details view to properties created by the logged-in lister:
            // if (property.ListerId != userId)
            //     return Forbid();

            // Return the shared Details view from Views/Properties/Details.cshtml
            return View("~/Views/Properties/Details.cshtml", property);
        }
        public async Task<IActionResult> ReservedProperties()
        {
            var userId = _userManager.GetUserId(User);
            var reservedProperties = await _context.Properties
                .Where(p => p.ListerId == userId && p.IsReserved)
                .Include(p => p.Buyer) // Load Buyer Information
                .ToListAsync();

            return View(reservedProperties);
        }
        [HttpGet]
        public async Task<IActionResult> Inquiries()
        {
            var userId = _userManager.GetUserId(User);  // Lister ID
                                                        // Query properties belonging to this Lister
            var listerProperties = await _context.Properties
                .Where(p => p.ListerId == userId)
                .Select(p => p.Id)
                .ToListAsync();

            // Fetch inquiries for those properties
            var inquiries = await _context.Inquiries
                .Where(i => listerProperties.Contains(i.PropertyId))
                .Include(i => i.Property)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return View(inquiries); // pass to a Lister/Inquiries.cshtml view
        }
    }
}
