using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eshop.Models
{
    [Table("PRODUCTS_CATEGORIES")]
    public class Category
    {
        [Key]
        [Column("CATEGORY_ID")]
        public int Id { get; set; }

        [Column("CATEGORY_NAME")]
        public string CategoryName { get; set; }

        // Navigation property for the many-to-many relationship
        public List<ProdCat> ProdCats { get; set; } = new List<ProdCat>();
    }
}
