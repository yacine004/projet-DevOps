using csharp_web.Data;
using csharp_web.Models;

namespace csharp_web.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Vérifier si la base est déjà peuplée
            if (context.Burgers.Any())
            {
                return; // La base est déjà initialisée
            }

            // Insérer les données
            var burgers = new Burger[]
            {
                new Burger { Nom = "Cheeseburger", Description = "Burger classique avec fromage", Prix = 5000.00, Image = "cheeseburger.jpg" },
                new Burger { Nom = "Big Mac", Description = "Burger premium avec double viande", Prix = 7000.00, Image = "bigmac.jpg" },
                new Burger { Nom = "Chicken Burger", Description = "Burger au poulet grillé", Prix = 6000.00, Image = "chicken.jpg" }
            };
            context.Burgers.AddRange(burgers);

            var complements = new Complement[]
            {
                new Complement { Nom = "Frites", Description = "Frites croustillantes", Prix = 2000.00, Image = "frites.jpg" },
                new Complement { Nom = "Riz", Description = "Riz parfumé", Prix = 1500.00, Image = "riz.jpg" },
                new Complement { Nom = "Sauce Ketchup", Description = "Sauce ketchup maison", Prix = 500.00, Image = "ketchup.jpg" },
                new Complement { Nom = "Coca Cola", Description = "Boisson gazeuse Coca Cola", Prix = 1500.00, Image = "coca.jpg" },
                new Complement { Nom = "Eau", Description = "Eau minérale", Prix = 1000.00, Image = "eau.jpg" }
            };
            context.Complements.AddRange(complements);

            var menus = new Menu[]
            {
                new Menu { Nom = "Menu Cheeseburger", Description = "Menu complet avec Cheeseburger", Prix = 8500.00, Image = "menu_cheese.jpg" },
                new Menu { Nom = "Menu Big Mac", Description = "Menu premium avec Big Mac", Prix = 10500.00, Image = "menu_bigmac.jpg" }
            };
            context.Menus.AddRange(menus);

            var clients = new Client[]
            {
                new Client { Nom = "Dupont", Prenom = "Jean", Telephone = "771234567" },
                new Client { Nom = "Martin", Prenom = "Marie", Telephone = "772345678" }
            };
            context.Clients.AddRange(clients);

            var zones = new Zone[]
            {
                new Zone { Nom = "Centre-ville", Prix = 2000.00 },
                new Zone { Nom = "Banlieue Nord", Prix = 2500.00 }
            };
            context.Zones.AddRange(zones);

            var livreurs = new Livreur[]
            {
                new Livreur { Nom = "Livreur1", Prenom = "Paul", Telephone = "773456789" },
                new Livreur { Nom = "Livreur2", Prenom = "Sophie", Telephone = "774567890" }
            };
            context.Livreurs.AddRange(livreurs);

            context.SaveChanges();

            // Assigner les zones après création
            var livreur1 = context.Livreurs.First(l => l.Nom == "Livreur1");
            var livreur2 = context.Livreurs.First(l => l.Nom == "Livreur2");
            var zone1 = context.Zones.First(z => z.Nom == "Centre-ville");
            var zone2 = context.Zones.First(z => z.Nom == "Banlieue Nord");

            livreur1.ZoneId = zone1.Id;
            livreur2.ZoneId = zone2.Id;

            context.SaveChanges();

            var gestionnaires = new Gestionnaire[]
            {
                new Gestionnaire { Nom = "Admin", Prenom = "Admin", Telephone = "775678901", Login = "admin", Password = "admin123" }
            };
            context.Gestionnaires.AddRange(gestionnaires);

            context.SaveChanges(); // Sauvegarder les entités de base

            var commandes = new Commande[]
            {
                new Commande { ClientId = 1, Etat = EtatCommande.Terminee, Date = new DateTime(2025, 12, 13), Type = TypeCommande.Livraison, ZoneId = 1, LivreurId = 1 },
                new Commande { ClientId = 2, Etat = EtatCommande.EnCours, Date = new DateTime(2025, 12, 13), Type = TypeCommande.SurPlace }
            };
            context.Commandes.AddRange(commandes);

            var ligneCommandes = new LigneCommande[]
            {
                new LigneCommande { Commande = commandes[0], BurgerId = 1, Quantite = 1 },
                new LigneCommande { Commande = commandes[0], MenuId = 1, ComplementId = 1, Quantite = 1 },
                new LigneCommande { Commande = commandes[0], MenuId = 1, ComplementId = 4, Quantite = 1 }
            };
            context.LigneCommandes.AddRange(ligneCommandes);

            context.SaveChanges();

            var paiements = new Paiement[]
            {
                new Paiement { CommandeId = commandes[0].Id, DatePaiement = new DateTime(2025, 12, 13), Montant = 8500.00, Methode = MethodePaiement.Wave },
                new Paiement { CommandeId = commandes[1].Id, DatePaiement = new DateTime(2025, 12, 13), Montant = 10500.00, Methode = MethodePaiement.OM }
            };
            context.Paiements.AddRange(paiements);
            context.SaveChanges();

            commandes[0].PaiementId = paiements[0].Id;
            commandes[1].PaiementId = paiements[1].Id;
            context.SaveChanges();

            context.SaveChanges();
        }
    }
}