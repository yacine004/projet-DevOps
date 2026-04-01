using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace csharp_web.Models
{
    public class Livreur
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

        public int? ZoneId { get; set; }

        [ForeignKey("ZoneId")]
        public Zone? Zone { get; set; }
    }
}