using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Eshop.Models
{
    [Table("PAYMENTS")]
    public class Payment
    {
        [Key]
        [Column("PAYMENT_ID")]
        public int Id { get; set; }

        [Column("PURCHASE_ID")]
        public int PurchaseId { get; set; }

        [Column("USER_ID")]
        public int UserId { get; set; }

        [Column("PAYMENT_METHOD")]
        public string PaymentMethod { get; set; }

        [Column("TRANSACTION_ID")]
        public string TransactionId { get; set; }

        [Column("AMOUNT")]
        public decimal Amount { get; set; }

        [Column("PAYMENT_DATE")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        // New field for Stripe tracking
        [Column("PAYMENT_INTENT_ID")]
        public string PaymentIntentId { get; set; }

        // Navigation properties
        public Purchase Purchase { get; set; }
        public User User { get; set; }
    }
}
}
