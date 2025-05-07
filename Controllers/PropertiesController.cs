using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaharlikaGrandEstate.Data;
using MaharlikaGrandEstate.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MaharlikaGrandEstate.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public PropertiesController(
            ApplicationDbContext context,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        // GET: Properties
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var properties = await _context.Properties
                .Where(p => p.ListerId == userId && !p.IsArchived) // Only show active listings
                .ToListAsync();
            return View(properties);
        }
        // GET: Properties/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Properties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Property property, List<IFormFile> imageFiles, IFormFile? DisplayPhoto)

        {
            // Guarantee we have a ListerId to avoid null issues:
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            property.ListerId = userId;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
                return View(property);
            }

            try
            {
                if (DisplayPhoto != null && DisplayPhoto.Length > 0)
                {
                    var displayPhotoFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "properties");

                    if (!Directory.Exists(displayPhotoFolder))
                        Directory.CreateDirectory(displayPhotoFolder);

                    // Declare displayPhotoName outside so we can use it after the stream
                    var displayPhotoName = $"{Guid.NewGuid()}_{Path.GetFileName(DisplayPhoto.FileName)}";
                    var displayPhotoPath = Path.Combine(displayPhotoFolder, displayPhotoName);

                    using (var stream = new FileStream(displayPhotoPath, FileMode.Create))
                    {
                        await DisplayPhoto.CopyToAsync(stream);
                    }

                    var displayImageUrl = Path.Combine("images", "properties", displayPhotoName)
                        .Replace("\\", "/");

                    property.ImageUrl = displayImageUrl;
                }

                if (imageFiles != null && imageFiles.Count > 0)
                {
                    bool isFirstImage = true;
                    foreach (var file in imageFiles)
                    {
                        if (file.Length > 0)
                        {
                            var uploadsFolder = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                "images",
                                "properties");

                            if (!Directory.Exists(uploadsFolder))
                                Directory.CreateDirectory(uploadsFolder);

                            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using var stream = new FileStream(filePath, FileMode.Create);
                            await file.CopyToAsync(stream);

                            var imageUrl = Path.Combine("images", "properties", uniqueFileName)
                                .Replace("\\", "/");

                            if (isFirstImage)
                            {
                                property.ImageUrl = imageUrl; // Main image
                                isFirstImage = false;
                            }

                            // Add each uploaded image to the Images collection
                            property.Images.Add(new PropertyImage { ImageUrl = imageUrl });
                        }
                    }
                }

                property.CreatedAt = DateTime.UtcNow;
                property.IsApproved = false;

                _context.Properties.Add(property);
                await _context.SaveChangesAsync();

                // Optionally notify admin
                string adminEmail = "malveze111@gmail.com";
                string subject = "New Property Listing Submitted";
                string message = $"A new property titled \"{property.Title}\" has been submitted and is pending approval.";
                await _emailSender.SendEmailAsync(adminEmail, subject, message);

                TempData["SuccessMessage"] = "Property saved successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", $"An error occurred while saving the property: {errorMessage}");
                return View(property);
            }
        }


        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var property = await _context.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (property == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            if (!User.IsInRole("Admin") && property.ListerId != userId)
            {
                return Forbid();
            }

            return View(property);
        }

        // POST: Properties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Title,Description,Price,Location,Bedrooms,Bathrooms,Size,YearBuilt,HasParking,FurnishingStatus,Latitude,Longitude")]
    Property property,
    IFormFile? DisplayPhoto,
            List<IFormFile>? NewImages)
        {
            if (id != property.Id)
                return NotFound();

            var existing = await _context.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (existing == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            if (!User.IsInRole("Admin") && existing.ListerId != userId)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(property);
            }

            try
            {
                if (NewImages != null && NewImages.Count > 0)
                {
                    foreach (var file in NewImages)
                    {
                        if (file.Length > 0)
                        {
                            var uploadsFolder = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                "images",
                                "properties");

                            if (!Directory.Exists(uploadsFolder))
                                Directory.CreateDirectory(uploadsFolder);

                            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using var stream = new FileStream(filePath, FileMode.Create);
                            await file.CopyToAsync(stream);

                            var imageUrl = Path.Combine("images", "properties", uniqueFileName)
                                .Replace("\\", "/");

                            // Set as main image only if existing.ImageUrl is empty
                            if (string.IsNullOrEmpty(existing.ImageUrl))
                            {
                                existing.ImageUrl = imageUrl;
                            }
                            // Add the image to the property
                            existing.Images.Add(new PropertyImage { ImageUrl = imageUrl });
                        }
                    }

                    // ✅ Delete old main image only if it was replaced
                    if (!string.IsNullOrEmpty(existing.ImageUrl))
                    {
                        var oldPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.ImageUrl);
                        if (System.IO.File.Exists(oldPhotoPath))
                        {
                            System.IO.File.Delete(oldPhotoPath);
                        }
                    }
                }


                // Update fields
                existing.Title = property.Title;
                existing.Description = property.Description;
                existing.Price = property.Price;
                existing.Location = property.Location;
                existing.Bedrooms = property.Bedrooms;
                existing.Bathrooms = property.Bathrooms;
                existing.Size = property.Size;
                existing.YearBuilt = property.YearBuilt;
                existing.HasParking = property.HasParking;
                existing.FurnishingStatus = property.FurnishingStatus;
                existing.Latitude = property.Latitude;
                existing.Longitude = property.Longitude;

                _context.Properties.Update(existing);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropertyExists(property.Id))
                    return NotFound();
                else
                    throw;
            }
        }


        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
        }

        // GET: /Properties/Details/5
        [Authorize(Roles = "Admin,Lister,Buyer")]
        public async Task<IActionResult> Details(int id)
        {
            var prop = await _context.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (prop == null)
                return NotFound();

            return View("Details", prop);
        }

        // GET: /Properties/Inquiries (for Listers)
        [Authorize(Roles = "Lister")]
        public async Task<IActionResult> Inquiries()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var listerProps = await _context.Properties
                .Where(p => p.ListerId == userId)
                .Select(p => p.Id)
                .ToListAsync();

            var inquiries = await _context.Inquiries
                .Where(i => listerProps.Contains(i.PropertyId))
                .Include(i => i.Property)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return View(inquiries);
        }

        public async Task<IActionResult> Archived()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var archivedProperties = await _context.Properties
                .Where(p => p.ListerId == userId && p.IsArchived)
                .ToListAsync();

            return View("Archived", archivedProperties); // ✅ Fix: Correct view name
        }

        [HttpPost]
        public async Task<IActionResult> Archive(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound();

            // Set property as archived
            property.IsArchived = true;
            _context.Properties.Update(property);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirect back to active properties list
        }
        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound();

            // Set property as active again
            property.IsArchived = false;
            _context.Properties.Update(property);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Archived)); // Redirect to archive page after restoring
        }

        [HttpPost]
        [Authorize]
        [Route("Properties/DeleteImage/{imageId}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var image = await _context.PropertyImages.FindAsync(imageId);
            if (image == null)
                return NotFound();

            var property = await _context.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == image.PropertyId);

            if (property == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            if (!User.IsInRole("Admin") && property.ListerId != userId)
                return Forbid();

            if (property.ImageUrl == image.ImageUrl)
            {
                property.ImageUrl = null; // Update the main image URL if the display photo is deleted
            }

            _context.PropertyImages.Remove(image);
            await _context.SaveChangesAsync();

            // Optionally, delete the actual file from the server
            var imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageUrl);
            if (System.IO.File.Exists(imageFilePath))
            {
                System.IO.File.Delete(imageFilePath);
            }

            return RedirectToAction(nameof(Edit), new { id = property.Id });
        }
        }
}
