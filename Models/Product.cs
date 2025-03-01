using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eshop.Models
{
    [Table("PRODUCTS")]
    public class Product
    {
        [Key]
        [Column("PRODUCT_ID")]
        public int Id { get; set; }

        [Column("STORE_ID")]
        public int StoreId { get; set; }

        [Column("PRODUCT_NAME")]
        public string Name { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Column("IMAGE")]
        public string Image { get; set; }

        [Column("PRICE")]
        public decimal Price { get; set; }

        [Column("STOCK_QUANTITY")]
        public int StockQuantity { get; set; }

        // Navigation property for the many-to-many relationship
        public List<ProdCat> ProdCats { get; set; } = new List<ProdCat>();
    }
}
