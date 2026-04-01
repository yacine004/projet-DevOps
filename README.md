# Mise en Contexte pour GitHub Copilot : Projet Brasil Burger - Transition C# (Client) vers Symfony (Gestionnaire)

## Vue d'ensemble du Projet
Ce projet est une application de commande de burgers en ligne, divisée en deux parties principales selon l'énoncé :
- **Partie Client** : Développée en **C# ASP.NET Core MVC** (déjà terminée). Elle permet aux utilisateurs de naviguer dans le catalogue, gérer leur panier, passer des commandes, et payer via Wave ou Orange Money.
- **Partie Gestionnaire** : À développer en **Symfony** (PHP). Elle sera utilisée par les administrateurs/gestionnaires pour gérer les produits (burgers, menus, compléments), les commandes, les clients, les livreurs, les zones de livraison, et les paiements. Cette partie doit être une interface web sécurisée avec authentification.

Le projet utilise **PostgreSQL** comme base de données commune. La partie C# a été déployée sur Render avec Docker, et la partie Symfony devra s'intégrer à la même BDD sans conflits.

## Modèles C# Créés (Entités)
Voici les entités principales créées en C# (dans le dossier `Models/`). Elles définissent la structure des données. Adaptez-les en entités Doctrine pour Symfony (avec annotations `@ORM`).

### 1. Client
- **Propriétés** :
  - `int Id` (clé primaire)
  - `string Nom` (requis, max 255)
  - `string Prenom` (requis, max 255)
  - `string Telephone` (requis, max 20, unique)
- **Relations** : Une commande appartient à un client (`Commande.ClientId`).

### 2. Commande
- **Propriétés** :
  - `int Id` (clé primaire)
  - `DateTime Date`
  - `EtatCommande Etat` (enum : EnCours, Validee, Terminee, Annulee)
  - `TypeCommande Type` (enum : SurPlace, AEmporter, Livraison)
  - `decimal Total`
  - `int ClientId` (clé étrangère vers Client)
  - `int? ZoneId` (clé étrangère vers Zone, nullable pour livraison)
  - `int? LivreurId` (clé étrangère vers Livreur, nullable)
  - `int? PaiementId` (clé étrangère vers Paiement, nullable)
- **Relations** : Appartient à Client, Zone, Livreur, Paiement. A plusieurs LigneCommande.

### 3. LigneCommande
- **Propriétés** :
  - `int Id` (clé primaire)
  - `int Quantite`
  - `int CommandeId` (clé étrangère vers Commande)
  - `int? BurgerId` (clé étrangère vers Burger, nullable)
  - `int? MenuId` (clé étrangère vers Menu, nullable)
  - `int? ComplementId` (clé étrangère vers Complement, nullable)
- **Relations** : Appartient à Commande, et à un produit (Burger, Menu, ou Complement).

### 4. Burger
- **Propriétés** :
  - `int Id` (clé primaire)
  - `string Nom` (requis, max 255)
  - `string Description` (requis)
  - `decimal Prix` (requis)
  - `string Image` (requis)
- **Relations** : Peut être dans plusieurs LigneCommande.

### 5. Menu
- **Propriétés** :
  - `int Id` (clé primaire)
  - `string Nom` (requis, max 255)
  - `string Description` (requis)
  - `decimal Prix` (requis)
  - `string Image` (requis)
- **Relations** : Peut être dans plusieurs LigneCommande.

### 6. Complement
- **Propriétés** :
  - `int Id` (clé primaire)
  - `string Nom` (requis, max 255)
  - `string Description` (requis)
  - `decimal Prix` (requis)
  - `string Image` (requis)
- **Relations** : Peut être dans plusieurs LigneCommande.

### 7. Zone
- **Propriétés** :
  - `int Id` (clé primaire)
  - `string Nom` (requis, max 255)
  - `decimal Prix` (prix de livraison)
- **Relations** : A plusieurs Livreur, utilisée dans Commande pour livraison.

### 8. Livreur
- **Propriétés** :
  - `int Id` (clé primaire)
  - `string Nom` (requis, max 255)
  - `string Prenom` (requis, max 255)
  - `string Telephone` (requis, max 20)
  - `int? ZoneId` (clé étrangère vers Zone, nullable)
- **Relations** : Appartient à Zone, assigné à Commande.

### 9. Gestionnaire
- **Propriétés** :
  - `int Id` (clé primaire)
  - `string Nom` (requis, max 255)
  - `string Prenom` (requis, max 255)
  - `string Telephone` (requis, max 20)
  - `string Login` (requis, max 255, unique)
  - `string Password` (requis, max 255, hashé)
- **Relations** : Utilisé pour l'authentification des admins (pas de relations directes avec autres entités).

### 10. Paiement
- **Propriétés** :
  - `int Id` (clé primaire)
  - `DateTime Date`
  - `MethodePaiement Methode` (enum : Wave, OM)
  - `decimal Montant`
  - `int CommandeId` (clé étrangère vers Commande)
- **Relations** : Appartient à Commande.

### Enums
- `EtatCommande` : EnCours, Validee, Terminee, Annulee
- `TypeCommande` : SurPlace, AEmporter, Livraison
- `MethodePaiement` : Wave, OM

## Schéma de la Base de Données (PostgreSQL)
La BDD est créée via Entity Framework Core (migrations). Voici le schéma des tables principales (colonnes, types, clés, contraintes). Utilisez cela pour créer les entités Doctrine en Symfony.

- **client** :
  - id (serial, primary key)
  - nom (varchar(255), not null)
  - prenom (varchar(255), not null)
  - telephone (varchar(20), not null, unique)

- **commande** :
  - id (serial, primary key)
  - date (timestamp, not null)
  - etat (integer, not null)  // 0=EnCours, 1=Validee, 2=Terminee, 3=Annulee
  - type (integer, not null)  // 0=SurPlace, 1=AEmporter, 2=Livraison
  - total (numeric, not null)
  - client_id (integer, foreign key to client.id)
  - zone_id (integer, foreign key to zone.id, nullable)
  - livreur_id (integer, foreign key to livreur.id, nullable)
  - paiement_id (integer, foreign key to paiement.id, nullable)

- **lignecommande** :
  - id (serial, primary key)
  - quantite (integer, not null)
  - commande_id (integer, foreign key to commande.id)
  - burger_id (integer, foreign key to burger.id, nullable)
  - menu_id (integer, foreign key to menu.id, nullable)
  - complement_id (integer, foreign key to complement.id, nullable)

- **burger** :
  - id (serial, primary key)
  - nom (varchar(255), not null)
  - description (text, not null)
  - prix (numeric, not null)
  - image (varchar(255), not null)

- **menu** : Même structure que burger.

- **complement** : Même structure que burger.

- **zone** :
  - id (serial, primary key)
  - nom (varchar(255), not null)
  - prix (numeric, not null)

- **livreur** :
  - id (serial, primary key)
  - nom (varchar(255), not null)
  - prenom (varchar(255), not null)
  - telephone (varchar(20), not null)
  - zone_id (integer, foreign key to zone.id, nullable)

- **gestionnaire** :
  - id (serial, primary key)
  - nom (varchar(255), not null)
  - prenom (varchar(255), not null)
  - telephone (varchar(20), not null)
  - login (varchar(255), not null, unique)
  - password (varchar(255), not null)  // Hashé avec bcrypt ou argon2

- **paiement** :
  - id (serial, primary key)
  - date (timestamp, not null)
  - methode (integer, not null)  // 0=Wave, 1=OM
  - montant (numeric, not null)
  - commande_id (integer, foreign key to commande.id)

**Remarques BDD** :
- Utilisez les mêmes noms de tables/colonnes pour éviter les conflits.
- Les migrations EF ont créé des index et contraintes automatiques.
- Pour Symfony, configurez Doctrine avec PostgreSQL (dans `.env` : `DATABASE_URL=postgresql://user:pass@host:port/db`).

## Fonctionnalités Déjà Implémentées en C# (Partie Client)
- **Catalogue** : Affichage des burgers, menus, compléments avec filtres.
- **Panier** : Ajout/suppression d'articles, calcul du total, choix du mode (sur place/à emporter/livraison), zone de livraison.
- **Commandes** : Passage de commande (avec authentification), affichage des commandes client, annulation, paiement (Wave/OM).
- **Authentification** : Connexion/inscription clients via session.
- **Paiement** : Simulation (pas d'intégration réelle), bouton désactivé si commande non validée.
- **UI Mobile** : Design responsive pour mobile.

## Directives pour la Partie Symfony (Gestionnaire)
- **Objectif** : Créer une interface admin pour gérer les données (CRUD complet pour toutes les entités).
- **Authentification** : Utilisez les entités Gestionnaire pour login/logout sécurisé (avec roles si besoin).
- **CRUD** :
  - Produits (Burger, Menu, Complement) : Ajouter/modifier/supprimer avec upload d'images.
  - Commandes : Voir toutes les commandes, changer état, assigner livreur, voir paiements.
  - Clients/Livreurs/Zones : Gestion complète.
  - Paiements : Historique et validation.
- **Technologies** : Symfony 6+, Doctrine, Twig, Bootstrap pour UI, EasyAdmin ou SonataAdmin pour accélérer le CRUD.
- **Sécurité** : Utilisez Symfony Security pour protéger les routes admin.
- **Intégration BDD** : Connectez-vous à la même BDD PostgreSQL (pas de duplication).
- **Éviter les Erreurs** : Respectez les types/relations des modèles C#. Testez les requêtes Doctrine pour éviter les conflits de clés étrangères.
- **Déploiement** : Préparez pour Docker/Render comme la partie C#.

Si vous avez des questions ou besoin d'ajustements, demandez ! Copiez ce texte dans Copilot pour commencer.