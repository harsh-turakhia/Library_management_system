using Library_management_system.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Data_Entity.Dto
{
    public class UserDto
    {
        public int UserId { get; set; }

        public string Name { get; set; }


        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }


        public string? Password { get; set; }


        public string PhoneNumber { get; set; }

        public string Address { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public string? RoleName { get; set; }   

        public int RoleId { get; set; }

        public List<Role> RoleList{ get; set; } = new List<Role>();


        public int PageSize { get; set; }

        public int CurrentPage { get; set; }

        public int TotalaPages { get; set; }
    }
}
