using LMS_Data_Entity.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Repository.Interface
{
    public interface ILib
    {
        LibHomePageDto GetLibPageData();

        List<UserDto> LibUserSearchHandler(string query);

        List<BooksDto> LibBookSearchHandler(string query);

        List<AssignedBooksDto> LibAssignBookSearchHandler(string query);

        AssignBookDto GetLibAssignBookData(int id);

        RegisterDto GetAddUserData();

        bool CheckEmailExist(string email);

        bool LibAddUserPost(RegisterDto registerDto, string loggedIn);

        AddEditBookDto GetAddBookData();

        (bool result, string message) LibAddBookPost(AddEditBookDto addEditBookDto, string loggedIn);

        (bool result, string message) LibAssignBookByBookPost(AssignBookDto assignBookDto , string loggedIn);
    }
}
