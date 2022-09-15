using GlobalShopping.Data.Entities;

namespace GlobalShopping.Models
{
    public class HomeViewModel
    {

        public ICollection<Product> Products { get; set; }

        public float Quantity { get; set; }

    }
}
