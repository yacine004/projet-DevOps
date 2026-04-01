using System.ComponentModel.DataAnnotations;

namespace csharp_web.Models
{
    public enum TypeProduit
    {
        Burger,
        Menu,
        Complement
    }

    public class PanierItem
    {
        public int Id { get; set; }
        public TypeProduit Type { get; set; }
        public string Nom { get; set; }
        public double Prix { get; set; }
        public int Quantite { get; set; }
        public string Image { get; set; }
    }
}