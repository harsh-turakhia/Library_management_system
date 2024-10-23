using System.ComponentModel.DataAnnotations;

namespace Library_management_system.Models
{
    public class User
    {
        public int UserId { get; set; }


        public string Username { get; set; }


        public string Email { get; set; }


        public string Password { get; set; }


        public string Address { get; set; }

        public int? Age { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public int Role { get; set; }

    }
}
