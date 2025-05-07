using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaharlikaGrandEstate.Data;
using MaharlikaGrandEstate.Models;
using MaharlikaGrandEstate.Services;
using Microsoft.AspNetCore.Identity;

namespace MaharlikaGrandEstate.Controllers
{
    [Authorize(Roles = "Buyer")]
    public class BuyerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly StripeService _stripeService;

        public BuyerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, StripeService stripeService)
        {
            _context = context;
            _userManager = userManager;
            _stripeService = stripeService;
        }

        // 🔹 Browse Available Properties
        public async Task<IActionResult> Browse(string searchString)
        {
            var properties = _context.Properties
                .Where(p => p.IsApproved && !p.IsSold);

            if (!string.IsNullOrEmpty(searchString))
            {
                properties = properties.Where(p => p.Title.Contains(searchString) || p.Location.Contains(searchString));
            }

            return View(await properties.ToListAsync());
        }

        // 🔹 View Property Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var property = await _context.Properties
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
                return NotFound();

            return View(property);
        }

        // 🔹 Redirect to Stripe Checkout for Reservation Payment
        public async Task<IActionResult> ReservePayment(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null || property.IsReserved || property.IsSold)
            {
                TempData["ErrorMessage"] = "This property is no longer available.";
                return RedirectToAction("Browse");
            }

            decimal reservationFee = property.Price * 0.10M; // 10% of Property Price
            string checkoutUrl = _stripeService.CreateCheckoutSession(reservationFee, id.ToString());

            return Redirect(checkoutUrl);
        }

        // 🔹 Handle Successful Payment and Reserve Property
        public async Task<IActionResult> PaymentSuccess(int propertyId)
        {
            var userId = _userManager.GetUserId(User);
            var property = await _context.Properties.FindAsync(propertyId);

            if (property == null || property.IsReserved || property.IsSold)
            {
                TempData["ErrorMessage"] = "This property is no longer available.";
                return RedirectToAction("Browse");
            }

            // Mark property as reserved
            property.IsReserved = true;
            property.ReservedById = userId;
            property.ReservedAt = DateTime.UtcNow;

            _context.Properties.Update(property);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reservation successful! The property is now reserved.";
            return RedirectToAction("MyReservations");
        }

        // 🔹 View Reserved Properties
        public async Task<IActionResult> MyReservations()
        {
            var userId = _userManager.GetUserId(User);
            var reservedProperties = await _context.Properties
                .Where(p => p.ReservedById == userId && p.IsReserved)
                .ToListAsync();

            return View(reservedProperties);
        }

        // 🔹 Cancel Reservation (Revert Reservation)
        [HttpPost]
        public async Task<IActionResult> RevertReservation(int propertyId)
        {
            var userId = _userManager.GetUserId(User);
            var property = await _context.Properties.FindAsync(propertyId);

            if (property == null || !property.IsReserved || property.ReservedById != userId)
            {
                TempData["ErrorMessage"] = "You cannot cancel this reservation.";
                return RedirectToAction("MyReservations");
            }

            property.IsReserved = false;
            property.ReservedById = null;
            property.ReservedAt = null;

            _context.Properties.Update(property);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reservation has been canceled.";
            return RedirectToAction("MyReservations");
        }

    }
}
