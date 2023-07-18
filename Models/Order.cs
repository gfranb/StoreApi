using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StoreApi.Models
{
    public class Order
    {
        [Key] 
        public int Id { get; set; }
        [Required]
        public List<Product> Products { get; set; }
        [Required]
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public Order()
        {
            Products = new List<Product>();
        }
    }
}
