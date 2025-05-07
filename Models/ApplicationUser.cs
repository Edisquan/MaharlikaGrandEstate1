using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser
{
    [Required] // Ensures that FullName must have a value
    public string FullName { get; set; } = string.Empty;  // ✅ FullName is required
}
