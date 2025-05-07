using MaharlikaGrandEstate.Models;
using System;
using System.ComponentModel.DataAnnotations;

public class Inquiry
{
    public int Id { get; set; }

    [Required]
    public int PropertyId { get; set; }

    public virtual Property? Property { get; set; }

    // The buyer's user ID (foreign key to AspNetUsers)
    [Required]
    public string? BuyerId { get; set; }

    [Required]
    [Display(Name = "Your Message")]
    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    
}
