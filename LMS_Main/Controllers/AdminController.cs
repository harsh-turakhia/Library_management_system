using AspNetCoreHero.ToastNotification.Abstractions;
using Library_management_system.Models;
using LMS_Data_Entity.Dto;
using LMS_Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LMS_Main.Controllers
{
    public class AdminController : Controller
    {
        private readonly INotyfService _notyf;
        private readonly IAdmin _admin;
        public AdminController(INotyfService notyf, IAdmin admin)
        {
            _notyf = notyf;
            _admin = admin;
        }


        #region Admin Dashboard

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index(int currentPage = 1)
        {
            try
            {
                var adminPageData = _admin.GetAdminPageData(currentPage);
                return View(adminPageData);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Admin");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AdminUserSearchHandler(string query)
        {
            try
            {
                var data = _admin.AdminUserSearchHandler(query);
                return Json(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Admin");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AdminBookSearchHandler(string query)
        {
            try
            {
                var data = _admin.AdminBookSearchHandler(query);
                return Json(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Admin");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AdminAssignBookSearchHandler(string query)
        {
            try
            {
                var data = _admin.AdminAssignBookSearchHandler(query);
                return Json(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Admin");
            }
        }

        #endregion


        #region Add User


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddUser()
        {
            try
            {
                var data = _admin.GetAddUserData();
                return View(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Admin");
            }
        }


        [HttpPost]
        public bool CheckEmailExist(string email)
        {
            return _admin.CheckEmailExist(email);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddUser(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            try
            {
                bool check = CheckEmailExist(registerDto.Email);

                if (check)
                {
                    _notyf.Error("Email already exist..!");
                    return View(registerDto);
                }
                else
                {
                    var loggedIn = "";
                    if (User.Identity.IsAuthenticated)
                    {
                        //loggedIn = Int32.Parse(User.FindFirst("UserId")?.Value);
                        loggedIn = User.FindFirst("UserId")?.Value;
                    }

                    var added = _admin.AdminAddUserPost(registerDto, loggedIn);

                    if (added)
                    {
                        _notyf.Success("User added successfully..!");
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        _notyf.Error("Error in adding the user..!");
                        return View(registerDto);
                    }
                }

            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View(registerDto);
            }
        }

        #endregion


        #region Add Book

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddBook()
        {
            try
            {
                var data = _admin.GetAddBookData();
                return View(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Admin");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddBook(AddEditBookDto addEditBookDto)
        {
            if (!ModelState.IsValid)
            {
                return View(addEditBookDto);
            }

            try
            {
                var loggedIn = "";
                if (User.Identity.IsAuthenticated)
                {
                    loggedIn = User.FindFirst("UserId")?.Value;
                }
                var added = _admin.AdminAddBookPost(addEditBookDto, loggedIn);

                if (added)
                {
                    _notyf.Success("Book added successfully..!");
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    _notyf.Error("Error in adding the book..!");
                    return View(addEditBookDto);
                }
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View(addEditBookDto);
            }
        }

        #endregion


        #region Remove Book

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AdminRemoveBook(int bookId)
        {
            try
            {
                var result = _admin.AdminRemoveBook(bookId);
                if (result)
                {
                    _notyf.Success("Book removed successfully!");
                    return Json(new { success = true });
                }
                else
                {
                    _notyf.Warning("Book could not be removed.");
                    return Json(new { success = false });
                }
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Admin");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AdminRemoveUser(int UserId)
        {
            try
            {
                var result = _admin.AdminRemoveUser(UserId);
                if (result)
                {
                    _notyf.Success("User removed successfully!");
                    return Json(new { success = true });
                }
                else
                {
                    _notyf.Warning("User could not be removed.");
                    return Json(new { success = false });
                }
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Admin");
            }
        }

        #endregion  


        #region Dropdown additions

        [Authorize(Roles = "Admin , Librarian")]
        [HttpPost]
        public List<Publications> AddPublication(string publicationName)
        {
            try
            {
                var data = _admin.AddPublicationPost(publicationName);
                return data;
            }
            catch
            {
                _notyf.Error("Publication could not be added. Please try again.");
                return null;
            }
        }


        [Authorize(Roles = "Admin , Librarian")]
        [HttpPost]
        public List<Language> AddLanguage(string languageName)
        {
            try
            {
                var data = _admin.AddLanguagePost(languageName);
                return data;
            }
            catch
            {
                _notyf.Error("Language could not be added. Please try again.");
                return null;
            }
        }


        [Authorize(Roles = "Admin , Librarian")]
        [HttpPost]
        public List<Authors> AddAuthor(string authorName)
        {
            try
            {
                var data = _admin.AddAuthorPost(authorName);
                return data;
            }
            catch
            {
                _notyf.Error("Author could not be added. Please try again.");
                return null;
            }
        }

        #endregion


        #region Edit User

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            try
            {
                var userData = _admin.GetEditUserData(id);
                return View(userData);
            }
            catch
            {
                _notyf.Error("Author could not be added. Please try again.");
                return RedirectToAction("Index", "Admin");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditUser(UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return View(userDto);
            }
            try
            {
                bool check = CheckEmailExist(userDto.Email);

                if (check)
                {
                    _notyf.Error("Email already exist!");
                    return RedirectToAction("EditUser", userDto.UserId);
                }
                else
                {
                    var updated = _admin.EditUserDataPost(userDto);
                    if (updated)
                    {
                        _notyf.Success("User updated successfully");
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        _notyf.Error("Error in updating user... Please try again!");
                        return RedirectToAction("Index", "Admin");
                    }
                }
            }
            catch
            {
                _notyf.Error("Author could not be added. Please try again.");
                return View(userDto);
            }
        }

        #endregion


        #region Edit Book

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditBook(int id)
        {
            try
            {
                var bookData = _admin.GetEditBookData(id);
                return View(bookData);
            }
            catch
            {
                _notyf.Error("Author could not be added. Please try again.");
                return RedirectToAction("Index", "Admin");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditBook(AddEditBookDto addEditBookDto)
        {
            if (!ModelState.IsValid)
            {
                return View(addEditBookDto);
            }

            try
            {
                var updated = _admin.EditBookDataPost(addEditBookDto);
                if (updated)
                {
                    _notyf.Success("Book updated successfully");
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    _notyf.Error("Error in updating book... Please try again!");
                    return RedirectToAction("EditBook", addEditBookDto.BookId);
                }
            }
            catch
            {
                _notyf.Error("Book could not be modified. Please try again.");
                return RedirectToAction("EditBook", addEditBookDto.BookId);
            }

        }

        #endregion


        #region Assign Book


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AssignBook()
        {
            try
            {
                var data = _admin.GetAssignBookData();
                return View(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Admin");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AssignBook(AssignBookDto assignBookDto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AssignBook", assignBookDto);
            }
            try
            {
                var loggedIn = "";
                if (User.Identity.IsAuthenticated)
                {
                    loggedIn = User.FindFirst("UserId")?.Value;
                }
                var assignBook = _admin.AdminAssignBookPost(assignBookDto, loggedIn);

                if (assignBook.result == true)
                {
                    _notyf.Success(assignBook.message);
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    _notyf.Error(assignBook.message ?? "An error occurred.");
                    return RedirectToAction("AssignBook", "Admin");
                }
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Admin");
            }
        }

        #endregion


        #region Return Book


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditAssignBook(int id)
        {
            try
            {
                var data = _admin.GetAssignedBookData(id);
                return View(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Lib");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditAssignBook(AssignedBooksDto assignedBooksDto)
        {
            try
            {
                var result = _admin.ReturnBookPost(assignedBooksDto);

                if (result.result = true)
                {
                    _notyf.Success(result.message);
                    return RedirectToAction("Index", "Admin");

                }
                else
                {
                    _notyf.Error(result.message);
                    return RedirectToAction("EditAssignBook", "Admin");

                }
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Admin");
            }
        }

        #endregion

    }
}
