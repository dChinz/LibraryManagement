using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public UseRole Role { get; set; } = UseRole.LIBRARIAN;

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
    public enum UseRole
    {
        ADMIN, 
        LIBRARIAN
    }
}
