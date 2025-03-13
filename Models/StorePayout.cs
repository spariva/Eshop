using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Eshop.Models
{
    [Table("STORE_PAYOUTS")]
    public class StorePayout
    {
        [Key]
        [Column("PAYOUT_ID")]
        public int Id { get; set; }

        [Column("STORE_ID")]
        public int StoreId { get; set; }

        [Column("PURCHASE_ID")]
        public int PurchaseId { get; set; }

        [Column("PAYOUT_AMOUNT")]
        public decimal PayoutAmount { get; set; }

        [Column("PAYOUT_STATUS")]
        public string PayoutStatus { get; set; } = "Pending"; // Default to Pending

        [Column("PAYOUT_DATE")]
        public DateTime PayoutDate { get; set; } = DateTime.Now;

        [Column("PAYOUT_METHOD")]
        public string PayoutMethod { get; set; }

        // Navigation properties
        public Store Store { get; set; }
        public Purchase Purchase { get; set; }
    }
}

