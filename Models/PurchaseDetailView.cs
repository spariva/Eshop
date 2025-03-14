namespace Eshop.Models
{
    public class PurchaseDetailView
    {
        public Purchase Purchase { get; set; }
        public List<PurchaseItem> PurchaseItems { get; set; }
        public List<Product> Products { get; set; }
    }
}
