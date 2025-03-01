using System.ComponentModel.DataAnnotations.Schema;

namespace Eshop.Models
{
    [Table("PROD_CAT")]
    public class ProdCat
    {
        [Column("PRODUCT_ID")]
        public int ProductId { get; set; }

        // Navigation property to the Product
        public Product Product { get; set; }

        [Column("CATEGORY_ID")]
        public int CategoryId { get; set; }

        // Navigation property to the Category
        public Category Category { get; set; }
    }
}