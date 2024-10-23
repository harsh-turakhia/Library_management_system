using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Data_Entity.Dto
{
    public class LibHomePageDto
    {
        public List<UserDto> UserList { get; set; } = new List<UserDto>();

        public List<BooksDto> BooksList { get; set; } = new List<BooksDto>();

        public List<AssignedBooksDto> AssignedBooksList { get; set; } = new List<AssignedBooksDto>();

        public int TotalUserCount { get; set; }

        public int TotalBooksCount { get; set; }

        public int TotalCopiesCount { get; set; }

        public int TotalAssignedCopies { get; set; }
    }
}
