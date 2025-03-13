using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Eshop.Models
{
    [Table("PURCHASE_VENDOR_MAPPING")]
    public class PurchaseVendorMapping
    {
        [Key]
        [Column("PURCHASE_VENDOR_ID")]
        public int Id { get; set; }

        [Column("PURCHASE_ID")]
        public int PurchaseId { get; set; }

        [Column("VENDOR_ID")]
        public int VendorId { get; set; }

        [Column("VENDOR_AMOUNT")]
        public decimal VendorAmount { get; set; }

        // Navigation properties
        public Purchase Purchase { get; set; }
        public Store Store { get; set; }
    }
}

