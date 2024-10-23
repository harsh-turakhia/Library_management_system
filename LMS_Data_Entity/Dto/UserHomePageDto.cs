using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Data_Entity.Dto;

public class UserHomePageDto
{
    public List<UserHomePageBooksDto> UserHomePageBooksList { get; set; }  = new List<UserHomePageBooksDto>();
}

public class UserHomePageBooksDto
{
    public int BookId { get; set; }

    public string Title { get; set; }

    public int AuthorId { get; set; }
    public string AuthorName { get; set; }

    public int PublicationId { get; set; }

    public string Publication { get; set; }

    public int LanguageId { get; set; }
    public string Language { get; set; }

    public int Copies { get; set; }

    public int? Price { get; set; }

    public int Pages { get; set; }

    public string ImageUrl { get; set; }

    public string Description { get; set; }      

}
