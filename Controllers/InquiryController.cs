using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaharlikaGrandEstate.Data;
using MaharlikaGrandEstate.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MaharlikaGrandEstate.Controllers
{
    [Authorize(Roles = "Buyer")] // Only buyers can send inquiries
    public class InquiryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public InquiryController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Inquiry/Create/5
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
                return NotFound();

            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound();

            // Pass property details to the view (optional)
            ViewBag.PropertyTitle = property.Title;
            ViewBag.PropertyId = property.Id;

            return View();
        }

      
        // POST: Inquiry/Create/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("Message")] Inquiry inquiry)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.PropertyTitle = property.Title;
                ViewBag.PropertyId = property.Id;
                return View(inquiry);
            }

            // Set these manually because they are required
            inquiry.PropertyId = property.Id;
            inquiry.BuyerId = _userManager.GetUserId(User);
            inquiry.CreatedAt = DateTime.UtcNow;

            _context.Inquiries.Add(inquiry);
            await _context.SaveChangesAsync();

            // Optionally email the lister
            var lister = await _userManager.FindByIdAsync(property.ListerId);
            if (!string.IsNullOrEmpty(lister?.Email))
            {
                string emailSubject = "New Inquiry for Your Property Listing";
                string emailMessage = $"You have received a new inquiry for your property '{property.Title}'.";
                await _emailSender.SendEmailAsync(lister.Email, emailSubject, emailMessage);
            }

            TempData["SuccessMessage"] = "Your inquiry has been submitted.";
            return RedirectToAction("Details", "Buyer", new { id = property.Id });
        }

    }
}
