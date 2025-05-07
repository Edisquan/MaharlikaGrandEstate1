using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaharlikaGrandEstate.Models
{
    public class PropertyImage
    {
        [Key]
        public int Id { get; set; }

        public int PropertyId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty; // Ensures non-null value

        [ForeignKey("PropertyId")]
        public virtual Property? Property { get; set; } // Nullable to prevent CS8618 warning
    }
}
