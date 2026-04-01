using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace csharp_web.Models
{
    public enum MethodePaiement
    {
        Wave,
        OM
    }

    public class Paiement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CommandeId { get; set; }

        [ForeignKey("CommandeId")]
        public Commande? Commande { get; set; }

        [Required]
        [Column("date_paiement")]
        public DateTime DatePaiement { get; set; }

        [Required]
        public double Montant { get; set; }

        [Required]
        public MethodePaiement Methode { get; set; }
    }
}