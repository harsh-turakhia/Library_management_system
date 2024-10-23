using Library_management_system.Models;
using LMS_Data_Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Repository.Interface
{
    public interface IAdmin
    {
        AdminHomePageDto GetAdminPageData(int currentPage);

        List<UserDto> AdminUserSearchHandler(string query);

        List<BooksDto> AdminBookSearchHandler(string query);

        List<AssignedBooksDto> AdminAssignBookSearchHandler(string query);

        RegisterDto GetAddUserData();

        bool CheckEmailExist(string email);

        bool AdminAddUserPost(RegisterDto registerDto, string loggedIn);

        AddEditBookDto GetAddBookData();

        bool AdminAddBookPost(AddEditBookDto addEditBookDto, string loggedIn);

        bool AdminRemoveUser(int UserId);

        bool AdminRemoveBook(int bookId);

        List<Publications> AddPublicationPost(string publicationName);

        List<Language> AddLanguagePost(string languageName);

        List<Authors> AddAuthorPost(string authorName);

        UserDto GetEditUserData(int id);

        bool EditUserDataPost(UserDto userDto);

        AddEditBookDto GetEditBookData(int id);

        bool EditBookDataPost(AddEditBookDto addEditBookDto);

        AssignBookDto GetAssignBookData();

        (bool result, string message) AdminAssignBookPost(AssignBookDto assignBookDto, string loggedIn);

        AssignedBooksDto GetAssignedBookData(int id);

        (bool result, string message) ReturnBookPost(AssignedBooksDto assignedBooksDto);
    }
}
