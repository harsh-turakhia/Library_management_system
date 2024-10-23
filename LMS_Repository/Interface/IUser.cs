using LMS_Data_Entity.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Repository.Interface
{
    public interface IUser
    {
        UserHomePageDto GetHomePageData();

        List<UserHomePageBooksDto> UserBookSearchHandler(string query);

        UserAssignedBooksDto GetMyBooks(string UserId);
    }
}
