using Microsoft.EntityFrameworkCore;
using csharp_web.Models;
using System.Threading;

namespace csharp_web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Burger> Burgers { get; set; }
        public DbSet<Complement> Complements { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Livreur> Livreurs { get; set; }
        public DbSet<Gestionnaire> Gestionnaires { get; set; }
        public DbSet<Paiement> Paiements { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<LigneCommande> LigneCommandes { get; set; }

        public override int SaveChanges()
        {
            ConvertDateTimeToUtc();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ConvertDateTimeToUtc();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ConvertDateTimeToUtc()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                // Convertir toutes les propriétés DateTime, pas seulement celles modifiées
                foreach (var property in entry.Properties)
                {
                    if (property.Metadata.ClrType == typeof(DateTime) || property.Metadata.ClrType == typeof(DateTime?))
                    {
                        var dateTimeValue = property.CurrentValue as DateTime?;
                        if (dateTimeValue.HasValue && dateTimeValue.Value.Kind != DateTimeKind.Utc)
                        {
                            property.CurrentValue = DateTime.SpecifyKind(dateTimeValue.Value, DateTimeKind.Utc);
                            // Marquer la propriété comme modifiée pour qu'elle soit incluse dans l'UPDATE
                            property.IsModified = true;
                        }
                    }
                }
            }
        }

        // Dictionnaires pour les conversions d'enums
        private static readonly Dictionary<EtatCommande, string> EtatToString = new()
        {
            { EtatCommande.EnCours, "En cours" },
            { EtatCommande.Validee, "Validee" },
            { EtatCommande.Terminee, "Terminee" },
            { EtatCommande.Annulee, "Annulee" }
        };

        private static readonly Dictionary<string, EtatCommande> StringToEtat = new()
        {
            { "En cours", EtatCommande.EnCours },
            { "Validée", EtatCommande.Validee },
            { "Validee", EtatCommande.Validee },
            { "Terminée", EtatCommande.Terminee },
            { "Terminee", EtatCommande.Terminee },
            { "Annulée", EtatCommande.Annulee },
            { "Annulee", EtatCommande.Annulee }
        };

        private static readonly Dictionary<TypeCommande, string> TypeToString = new()
        {
            { TypeCommande.SurPlace, "Sur place" },
            { TypeCommande.AEmporter, "A emporter" },
            { TypeCommande.Livraison, "Livraison" }
        };

        private static readonly Dictionary<string, TypeCommande> StringToType = new()
        {
            { "Sur place", TypeCommande.SurPlace },
            { "A emporter", TypeCommande.AEmporter },
            { "Livraison", TypeCommande.Livraison }
        };

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurations pour correspondre aux noms de tables et colonnes du script SQL (minuscules)
            modelBuilder.Entity<Burger>().ToTable("burger");
            modelBuilder.Entity<Burger>().Property(b => b.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Burger>().Property(b => b.Nom).HasColumnName("nom");
            modelBuilder.Entity<Burger>().Property(b => b.Description).HasColumnName("description");
            modelBuilder.Entity<Burger>().Property(b => b.Prix).HasColumnName("prix");
            modelBuilder.Entity<Burger>().Property(b => b.Image).HasColumnName("image");

            modelBuilder.Entity<Complement>().ToTable("complement");
            modelBuilder.Entity<Complement>().Property(c => c.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Complement>().Property(c => c.Nom).HasColumnName("nom");
            modelBuilder.Entity<Complement>().Property(c => c.Description).HasColumnName("description");
            modelBuilder.Entity<Complement>().Property(c => c.Prix).HasColumnName("prix");
            modelBuilder.Entity<Complement>().Property(c => c.Image).HasColumnName("image");

            modelBuilder.Entity<Menu>().ToTable("menu");
            modelBuilder.Entity<Menu>().Property(m => m.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Menu>().Property(m => m.Nom).HasColumnName("nom");
            modelBuilder.Entity<Menu>().Property(m => m.Description).HasColumnName("description");
            modelBuilder.Entity<Menu>().Property(m => m.Prix).HasColumnName("prix");
            modelBuilder.Entity<Menu>().Property(m => m.Image).HasColumnName("image");

            modelBuilder.Entity<Client>().ToTable("client");
            modelBuilder.Entity<Client>().Property(c => c.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Client>().Property(c => c.Nom).HasColumnName("nom");
            modelBuilder.Entity<Client>().Property(c => c.Prenom).HasColumnName("prenom");
            modelBuilder.Entity<Client>().Property(c => c.Telephone).HasColumnName("telephone");

            modelBuilder.Entity<Zone>().ToTable("zone");
            modelBuilder.Entity<Zone>().Property(z => z.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Zone>().Property(z => z.Nom).HasColumnName("nom");
            modelBuilder.Entity<Zone>().Property(z => z.Prix).HasColumnName("prix");

            modelBuilder.Entity<Livreur>().ToTable("livreur");
            modelBuilder.Entity<Livreur>().Property(l => l.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Livreur>().Property(l => l.Nom).HasColumnName("nom");
            modelBuilder.Entity<Livreur>().Property(l => l.Prenom).HasColumnName("prenom");
            modelBuilder.Entity<Livreur>().Property(l => l.Telephone).HasColumnName("telephone");
            modelBuilder.Entity<Livreur>().Property(l => l.ZoneId).HasColumnName("zone_id");

            // Configuration de la relation Livreur - Zone
            modelBuilder.Entity<Livreur>()
                .HasOne(l => l.Zone)
                .WithMany(z => z.Livreurs)
                .HasForeignKey(l => l.ZoneId);

            modelBuilder.Entity<Gestionnaire>().ToTable("gestionnaire");
            modelBuilder.Entity<Gestionnaire>().Property(g => g.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Gestionnaire>().Property(g => g.Nom).HasColumnName("nom");
            modelBuilder.Entity<Gestionnaire>().Property(g => g.Prenom).HasColumnName("prenom");
            modelBuilder.Entity<Gestionnaire>().Property(g => g.Telephone).HasColumnName("telephone");
            modelBuilder.Entity<Gestionnaire>().Property(g => g.Login).HasColumnName("login");
            modelBuilder.Entity<Gestionnaire>().Property(g => g.Password).HasColumnName("password");

            modelBuilder.Entity<Paiement>().ToTable("paiement");
            modelBuilder.Entity<Paiement>().Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Paiement>().Property(p => p.CommandeId).HasColumnName("commande_id");
            modelBuilder.Entity<Paiement>().Property(p => p.DatePaiement).HasColumnName("date_paiement");
            modelBuilder.Entity<Paiement>().Property(p => p.Montant).HasColumnName("montant");
            modelBuilder.Entity<Paiement>().Property(p => p.Methode).HasColumnName("methode");

            modelBuilder.Entity<Commande>().ToTable("commande");
            modelBuilder.Entity<Commande>().Property(c => c.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Commande>().Property(c => c.ClientId).HasColumnName("client_id");
            modelBuilder.Entity<Commande>().Property(c => c.Etat).HasColumnName("etat");
            modelBuilder.Entity<Commande>().Property(c => c.Date).HasColumnName("date_commande");
            modelBuilder.Entity<Commande>().Property(c => c.Type).HasColumnName("type");
            modelBuilder.Entity<Commande>().Property(c => c.ZoneId).HasColumnName("zone_id");
            modelBuilder.Entity<Commande>().Property(c => c.LivreurId).HasColumnName("livreur_id");
            modelBuilder.Entity<Commande>().Property(c => c.PaiementId).HasColumnName("paiement_id");

            modelBuilder.Entity<LigneCommande>().ToTable("ligne_commande");
            modelBuilder.Entity<LigneCommande>().Property(l => l.Id).HasColumnName("id").ValueGeneratedOnAdd();
            modelBuilder.Entity<LigneCommande>().Property(l => l.CommandeId).HasColumnName("commande_id");
            modelBuilder.Entity<LigneCommande>().Property(l => l.BurgerId).HasColumnName("burger_id");
            modelBuilder.Entity<LigneCommande>().Property(l => l.MenuId).HasColumnName("menu_id");
            modelBuilder.Entity<LigneCommande>().Property(l => l.ComplementId).HasColumnName("complement_id");
            modelBuilder.Entity<LigneCommande>().Property(l => l.Quantite).HasColumnName("quantite");

            // Conversions pour les enums
            modelBuilder.Entity<Commande>()
                .Property(c => c.Etat)
                .HasConversion(
                    etat => EtatToString[etat],
                    str => StringToEtat[str]
                );

            modelBuilder.Entity<Commande>()
                .Property(c => c.Type)
                .HasConversion(
                    type => TypeToString[type],
                    str => StringToType[str]
                );

            modelBuilder.Entity<Paiement>()
                .Property(p => p.Methode)
                .HasConversion<string>();

            // Configuration globale pour les DateTime - forcer UTC
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
                            v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)) : v,
                            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v));
                    }
                }
            }
        }
    }
}