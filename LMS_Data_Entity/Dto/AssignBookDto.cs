using Library_management_system.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Data_Entity.Dto
{
    public class AssignBookDto
    {

        public List<UserDto> UserList { get; set; } = new List<UserDto>();

        [Required(ErrorMessage = "User is required.")]
        public int UserId { get; set; }

        public List<BooksDto> BooksList { get; set; } = new List<BooksDto>();


        [Required(ErrorMessage = "Book is required.")]
        public int BookId { get; set; }
        
        public string? BookName { get; set; }


        [Required(ErrorMessage = "Issued date is required.")]
        [DataType(DataType.Date)]
        public DateOnly IssuedDate { get; set; }


        [Required(ErrorMessage = "Return date is required.")]
        [DataType(DataType.Date)]
        public DateOnly ReturnDate { get; set; }

        public int StatusId { get; set; }

        public int StatusName { get; set; }

        public List<Status> StatusList { get; set; } = new List<Status>();
    }
}
