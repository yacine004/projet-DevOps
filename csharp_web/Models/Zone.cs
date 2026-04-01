using System.ComponentModel.DataAnnotations;

namespace csharp_web.Models
{
    public class Zone
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Nom { get; set; }

        [Required]
        public double Prix { get; set; }

        public ICollection<Livreur> Livreurs { get; set; } = new List<Livreur>();
    }
}