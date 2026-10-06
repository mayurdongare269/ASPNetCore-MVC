using System.ComponentModel.DataAnnotations;

namespace FIrstMVCWebApp.Dto
{
    public class UserDto
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
