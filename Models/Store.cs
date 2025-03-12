using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eshop.Models
{
    [Table("STORES")]
    public class Store
    {
        [Key]
        [Column("STORE_ID")]
        public int Id { get; set; }

        [Column("STORE_NAME")]
        public string Name { get; set; }

        [Column("EMAIL")]
        public string Email { get; set; }

        [Column("IMAGE")]
        public string Image { get; set; }

        [Column("CATEGORY")]
        public string Category { get; set; }
        [Column("USER_ID")]
        public int UserId { get; set; }
        [Column("STRIPE_ID")]
        public string StripeId { get; set; }
    }
}