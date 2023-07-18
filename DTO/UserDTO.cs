using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace StoreApi.DTO
{
    public class UserDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
    }
}
