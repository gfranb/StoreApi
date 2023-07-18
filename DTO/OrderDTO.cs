using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using StoreApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StoreApi.DTO
{
    public class OrderDTO
    {
        [ValidateNever]
        [JsonIgnore]
        public int UserID { get; set; }
        [Required]
        public List<Product> Products { get; set; }
    }
}
