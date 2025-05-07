using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaharlikaGrandEstate.Models
{
    public class Property
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string ListerId { get; set; } = string.Empty; // Foreign key for Lister

        public bool IsApproved { get; set; } = false; // Approval flag

        public bool IsSold { get; set; } = false;  // NEW: Track if the property is sold
                                                   // Buyer Information (Nullable)
        public string? BuyerId { get; set; }  // Nullable Buyer ID to avoid FK conflicts

        [ForeignKey("BuyerId")]
        public ApplicationUser? Buyer { get; set; }  // Establish Navigation Property
        public bool IsArchived { get; set; } = false;

        [Range(0, int.MaxValue, ErrorMessage = "Enter a valid number of bedrooms.")]
        public int Bedrooms { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Enter a valid number of bathrooms.")]
        public int Bathrooms { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Enter a valid property size.")]
        public double? Size { get; set; }

        [Range(1800, 2100, ErrorMessage = "Enter a valid year built.")]
        public int? YearBuilt { get; set; }

        public bool HasParking { get; set; } = false;

        [Required]
        public string FurnishingStatus { get; set; } = "Unfurnished";

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Fallback image if no multiple images are uploaded
        public string? ImageUrl { get; set; }

        public string? DisplayPhotoUrl { get; set; }

        // Collection for multiple images
        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();

        // Reservation Fields
        public bool IsReserved { get; set; } = false;
        public string? ReservedById { get; set; }
        public DateTime? ReservedAt { get; set; }

        // 🔹 ADD Reservation Fee (10% of Property Price)
        public decimal ReservationFee => Price * 0.10M;
        

    }
}
