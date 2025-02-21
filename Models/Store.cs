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
        public string StoreName { get; set; }

        [Column("EMAIL")]
        public string Email { get; set; }

        [Column("IMAGE")]
        public string Image { get; set; }

        [Column("CATEGORY")]
        public string Category { get; set; }

        [Column("PASSWORD_HASH")]
        public string PasswordHash { get; set; }

        [Column("SALT")]
        public string Salt { get; set; }

        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; }

        [Column("UPDATED_AT")]
        public DateTime UpdatedAt { get; set; }
    }
}