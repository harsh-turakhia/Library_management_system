using Library_management_system.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Data_Entity.Dto
{
    public class AddEditUserDto
    {
        public int? UserId { get; set; }
        [Required(ErrorMessage = "Name is Required")]

        public string Name { get; set; }


        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; }

        public string? Password { get; set; }


        [Required(ErrorMessage = "Phone Number is Required")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }


        public int? RoleId { get; set; }


        public string? RoleName { get; set; }


        public List<Role> RoleList { get; set; } = new List<Role>();
    }
}
