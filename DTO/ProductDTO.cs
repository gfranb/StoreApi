using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTO
{
    public class ProductDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public double Price { get; set; }
    }
}
