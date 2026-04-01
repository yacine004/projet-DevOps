using csharp_web.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace csharp_web.Services
{
    public interface IPanierService
    {
        void AddToCart(TypeProduit type, int id, string nom, double prix, string image);
        void RemoveFromCart(int itemId);
        void UpdateQuantity(int itemId, int quantity);
        List<PanierItem> GetCartItems();
        int GetCartItemCount();
        double GetTotal();
        void ClearCart();
        void SetModeConsommation(string mode);
        string GetModeConsommation();
        void SetMethodePaiement(MethodePaiement methode);
        MethodePaiement GetMethodePaiement();
        void SetZoneLivraison(int zoneId);
        int? GetZoneLivraison();
    }

    public class PanierService : IPanierService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "CartItems";
        private const string ModeSessionKey = "ModeConsommation";
        private const string PaiementSessionKey = "MethodePaiement";
        private const string ZoneSessionKey = "ZoneLivraison";

        public PanierService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void AddToCart(TypeProduit type, int id, string nom, double prix, string image)
        {
            var cart = GetCartItems();
            var existingItem = cart.FirstOrDefault(i => i.Type == type && i.Id == id);

            if (existingItem != null)
            {
                existingItem.Quantite++;
            }
            else
            {
                cart.Add(new PanierItem
                {
                    Id = id,
                    Type = type,
                    Nom = nom,
                    Prix = prix,
                    Quantite = 1,
                    Image = image
                });
            }

            SaveCart(cart);
        }

        public void RemoveFromCart(int itemId)
        {
            var cart = GetCartItems();
            cart.RemoveAll(i => i.Id == itemId);
            SaveCart(cart);
        }

        public void UpdateQuantity(int itemId, int quantity)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.Quantite = quantity;
                }
            }
            SaveCart(cart);
        }

        public List<PanierItem> GetCartItems()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return new List<PanierItem>();

            var cartJson = session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson) ? new List<PanierItem>() : JsonSerializer.Deserialize<List<PanierItem>>(cartJson) ?? new List<PanierItem>();
        }

        public int GetCartItemCount()
        {
            return GetCartItems().Sum(i => i.Quantite);
        }

        public double GetTotal()
        {
            return GetCartItems().Sum(i => i.Prix * i.Quantite);
        }

        public void ClearCart()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.Remove(CartSessionKey);
            }
        }

        public void SetModeConsommation(string mode)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetString(ModeSessionKey, mode);
            }
        }

        public string GetModeConsommation()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session?.GetString(ModeSessionKey) ?? "Sur place";
        }

        public void SetMethodePaiement(MethodePaiement methode)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetString(PaiementSessionKey, methode.ToString());
            }
        }

        public MethodePaiement GetMethodePaiement()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var methodeStr = session?.GetString(PaiementSessionKey);
            return Enum.TryParse<MethodePaiement>(methodeStr, out var methode) ? methode : MethodePaiement.Wave;
        }

        public void SetZoneLivraison(int zoneId)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetInt32(ZoneSessionKey, zoneId);
            }
        }

        public int? GetZoneLivraison()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session?.GetInt32(ZoneSessionKey);
        }

        private void SaveCart(List<PanierItem> cart)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var cartJson = JsonSerializer.Serialize(cart);
                session.SetString(CartSessionKey, cartJson);
            }
        }
    }
}