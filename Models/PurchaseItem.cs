using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Eshop.Models
{
    [Table("PURCHASE_ITEMS")]
    public class PurchaseItem
    {
        [Key]
        [Column("PURCHASE_ITEM_ID")]
        public int Id { get; set; }

        [Column("PURCHASE_ID")]
        public int PurchaseId { get; set; }

        [Column("PRODUCT_ID")]
        public int ProductId { get; set; }

        [Column("QUANTITY")]
        public int Quantity { get; set; }

        [Column("PRICE_AT_PURCHASE")]
        public decimal PriceAtPurchase { get; set; }

        // Navigation properties
        public Purchase Purchase { get; set; }
        public Product Product { get; set; }
    }
}

