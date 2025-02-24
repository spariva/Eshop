using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eshop.Models
{
    [Table("USERS")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("USER_ID")]
        public int Id { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
        [Column("EMAIL")]
        public string Email { get; set; }
        [Column("PASSWORD_HASH")]
        public string PasswordHash { get; set; }
        [Column("SALT")]
        public string Salt { get; set; }
        [Column("TELEPHONE")]
        public string Telephone { get; set; }
        [Column("ADDRESS")]
        public string Address { get; set; }
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; }
        [Column("UPDATE_AT")]
        public DateTime UpdatedAt { get; set; }

    }
}
