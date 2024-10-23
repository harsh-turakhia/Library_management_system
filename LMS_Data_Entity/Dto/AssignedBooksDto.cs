using Library_management_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Data_Entity.Dto
{
    public class AssignedBooksDto
    {
        public int AssignedId { get; set; }

        public int BookId { get; set; }

        public string? BookName { get; set; }

        public int UserId { get; set; }

        public string? UserName { get; set; }

        public int? IssuedById { get; set; }

        public string? IssuedByName { get; set; }

        public DateOnly IssuedDate { get; set; }

        public DateOnly ReturnDate { get; set; }

        public DateOnly? ReturnedOn { get; set; }

        public int Status { get; set; }

        public string StatusName { get; set; }

        public List<Status> StatusList { get; set; } = new List<Status>();
    }
}
