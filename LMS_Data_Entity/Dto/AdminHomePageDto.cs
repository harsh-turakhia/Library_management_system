using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Data_Entity.Dto
{
    public class AdminHomePageDto
    {
        public List<UserDto> UserList { get; set; } = new List<UserDto>();        

        public List<BooksDto> BooksList { get; set; } = new List<BooksDto>();        

        public List<AssignedBooksDto> AssignedBooksList { get; set; } = new List<AssignedBooksDto>();        

        public int AllUserCount { get; set; }

        public int AllBookCount { get; set; }

        public int AllPublicationCount { get; set; }

        public int AllAuthorCount { get; set; }

        public int AllBooksCount { get; set; }

        public int AllCopiesCount { get; set; }

        public int AllAssignedBookCount { get; set; }


        // New properties for pagination
        public int UserCountForPagi { get; set; }
        public int BookCountForPagi { get; set; }
        public int AssignedBookCountForPagi { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
