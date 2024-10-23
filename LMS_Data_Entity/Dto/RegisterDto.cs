using System.ComponentModel.DataAnnotations;
using Library_management_system.Models;

namespace LMS_Data_Entity.Dto
{
    public class RegisterDto
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "FirstName Accepts Only Text Characters")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Email is Required")]
        [RegularExpression(@"^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }



        [Required(ErrorMessage = "Password is Required")]     
        public string Password { get; set; }


        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone Number should be of 10 Numbers")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please Enter Valid Phone Number")]
        [Required(ErrorMessage = "Phone Number is Required")]
        public string? PhoneNumber { get; set; }


        [Required(ErrorMessage = "Address is Required")]
        public string? Address { get; set; }


        public int? Role { get; set; }


        public List<Role> RoleList { get; set; } = new List<Role>();


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
