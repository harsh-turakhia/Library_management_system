using Library_management_system.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Data_Entity.Dto
{
    public class AddEditBookDto
    {
        public int BookId { get; set; }

        public string Title { get; set; }

        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }

        public List<Authors> AuthorsList{ get; set; } = new List<Authors> { };

        public int PublicationId { get; set; }

        public string? PublicationName { get; set; }

        public List<Publications> PublicationsList { get; set; } = new List<Publications> { };
        public int LanguageId { get; set; }

        public string? LanguageName { get; set; }

        public List<Language> LanguageList { get; set; } = new List<Language> { };

        public int Copies { get; set; }

        public int Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int IsDeleted { get; set; }
    }
}
