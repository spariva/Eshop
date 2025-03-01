using NuGet.Protocol;

namespace Eshop.Models
{
    public class StoreView
    {
        public Store Store { get; set; }
        public List<Product> Products { get; set; }
        public List<String> ProdCategories { get; set; }

    }
}