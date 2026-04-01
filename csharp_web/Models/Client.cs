using System.ComponentModel.DataAnnotations;

namespace csharp_web.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Nom { get; set; }

        [Required]
        [StringLength(255)]
        public string Prenom { get; set; }

        [Required]
        [StringLength(20)]
        public string Telephone { get; set; }
    }
}