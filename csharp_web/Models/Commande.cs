using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace csharp_web.Models
{
    public enum EtatCommande
    {
        EnCours,
        Validee,
        Terminee,
        Annulee
    }

    public enum TypeCommande
    {
        SurPlace,
        AEmporter,
        Livraison
    }

    public class Commande
    {
        [Key]
        public int Id { get; set; }

        public int? ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        [Required]
        public EtatCommande Etat { get; set; } = EtatCommande.EnCours;

        [Required]
        [Column("date_commande")]
        public DateTime Date { get; set; }

        [NotMapped]
        public double Total { get; set; }

        [Required]
        public TypeCommande Type { get; set; }

        public int? ZoneId { get; set; }

        [ForeignKey("ZoneId")]
        public Zone? Zone { get; set; }

        public int? LivreurId { get; set; }

        [ForeignKey("LivreurId")]
        public Livreur? Livreur { get; set; }

        public int? PaiementId { get; set; }

        [ForeignKey("PaiementId")]
        public Paiement? Paiement { get; set; }
    }
}