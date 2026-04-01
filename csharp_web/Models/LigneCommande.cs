using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace csharp_web.Models
{
    public class LigneCommande
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CommandeId { get; set; }

        [ForeignKey("CommandeId")]
        public Commande? Commande { get; set; }

        public int? BurgerId { get; set; }

        [ForeignKey("BurgerId")]
        public Burger? Burger { get; set; }

        public int? MenuId { get; set; }

        [ForeignKey("MenuId")]
        public Menu? Menu { get; set; }

        public int? ComplementId { get; set; }

        [ForeignKey("ComplementId")]
        public Complement? Complement { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantite { get; set; }

        // Propriété calculée pour le prix unitaire (non persistée)
        [NotMapped]
        public double PrixUnitaire
        {
            get
            {
                if (Burger != null) return Burger.Prix;
                if (Menu != null) return Menu.Prix;
                if (Complement != null) return Complement.Prix;
                return 0;
            }
        }
    }
}