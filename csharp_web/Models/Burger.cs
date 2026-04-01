using System.ComponentModel.DataAnnotations;

namespace csharp_web.Models
{
    public class Burger
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Nom { get; set; }

        public string Description { get; set; }

        [Required]
        public double Prix { get; set; }

        [StringLength(255)]
        public string Image { get; set; }
    }
}