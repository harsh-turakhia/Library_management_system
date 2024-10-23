using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Data_Entity.Dto
{
    public class UserAssignedBooksDto
    {
        public List<AssignedBooksDto> AssignedBooksList { get; set; } = new List<AssignedBooksDto> { };
    }
}
