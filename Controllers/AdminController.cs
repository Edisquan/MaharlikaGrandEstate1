using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaharlikaGrandEstate.Data;
using MaharlikaGrandEstate.Models;

namespace MaharlikaGrandEstate.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // Inject UserManager

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var pendingPropertiesCount = await _context.Properties.CountAsync(p => !p.IsApproved);
            var totalProperties = await _context.Properties.CountAsync();
            var totalUsers = await _context.Users.CountAsync();

            var adminCount = await _userManager.GetUsersInRoleAsync("Admin");
            var listerCount = await _userManager.GetUsersInRoleAsync("Lister");
            var buyerCount = await _userManager.GetUsersInRoleAsync("Buyer");

            ViewBag.PendingPropertiesCount = pendingPropertiesCount;
            ViewBag.TotalProperties = totalProperties;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.AdminCount = adminCount.Count;
            ViewBag.ListerCount = listerCount.Count;
            ViewBag.BuyerCount = buyerCount.Count;

            return View();
        }

        // GET: Admin/PendingProperties
        public async Task<IActionResult> PendingProperties()
        {
            var pending = await _context.Properties
                .Where(p => !p.IsApproved)
                .ToListAsync();
            return View(pending);
        }

        // GET: Admin/ApproveProperty/5
        public async Task<IActionResult> ApproveProperty(int? id)
        {
            if (id == null)
                return NotFound();

            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound();

            property.IsApproved = true;
            _context.Update(property);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PendingProperties));
        }
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApprovedProperties()
        {
            // Fetch properties where IsApproved is true
            var approvedProperties = await _context.Properties
                .Where(p => p.IsApproved)
                .ToListAsync();

            return View(approvedProperties);
        }
    }
}
