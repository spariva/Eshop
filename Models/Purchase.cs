using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Eshop.Models
{
    [Table("PURCHASES")]
    public class Purchase
    {

        [Key]
        [Column("PURCHASE_ID")]
        public int Id { get; set; }

        [Column("USER_ID")]
        public int UserId { get; set; }

        [Column("TOTAL_PRICE")]
        public decimal TotalPrice { get; set; }

        [Column("PAYMENT_STATUS")]
        public string PaymentStatus { get; set; } = "Completed"; // Default to Completed since we only record successful payments

        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // New field for Stripe tracking
        [Column("STRIPE_SESSION_ID")]
        public string StripeSessionId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public List<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
        public List<PurchaseVendorMapping> PurchaseVendorMappings { get; set; } = new List<PurchaseVendorMapping>();
    }
}

