using AspNetCoreHero.ToastNotification.Abstractions;
using LMS_Data_Entity.Dto;
using LMS_Repository.Interface;
using LMS_Repository.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS_Main.Controllers
{
    public class LibController : Controller
    {

        private readonly INotyfService _notyf;
        private readonly ILib _lib;
        private readonly IAdmin _admin;
        public LibController(INotyfService notyf, ILib lib, IAdmin admin)
        {
            _notyf = notyf;
            _lib = lib;
            _admin = admin;
        }

        #region Librarian Dashboard

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var libPageData = _lib.GetLibPageData();
                return View(libPageData);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Lib");
            }
        }


        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public IActionResult AdminUserSearchHandler(string query)
        {
            try
            {
                var data = _lib.LibUserSearchHandler(query);
                return Json(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Lib");
            }
        }


        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public IActionResult AdminBookSearchHandler(string query)
        {
            try
            {
                var data = _lib.LibBookSearchHandler(query);
                return Json(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Lib");
            }
        }


        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public IActionResult AdminAssignBookSearchHandler(string query)
        {
            try
            {
                var data = _lib.LibAssignBookSearchHandler(query);
                return Json(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Lib");
            }
        }


        #endregion


        #region Librarian Dashboard


        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public IActionResult AddUser()
        {
            try
            {
                var data = _lib.GetAddUserData();
                return View(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Lib");
            }
        }


        [HttpPost]
        public bool CheckEmailExist(string email)
        {
            return _lib.CheckEmailExist(email);
        }

        [Authorize(Roles = "Librarian")]
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
                        loggedIn = User.FindFirst("UserId")?.Value;
                    }
                    var added = _lib.LibAddUserPost(registerDto, loggedIn);

                    if (added)
                    {
                        _notyf.Success("User added successfully..!");
                        return RedirectToAction("Index", "Lib");
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


        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public IActionResult AddBook()
        {
            try
            {
                var data = _lib.GetAddBookData();
                return View(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Lib");
            }
        }


        [Authorize(Roles = "Librarian")]
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
                var added = _lib.LibAddBookPost(addEditBookDto, loggedIn);

                if (added.result)
                {
                    _notyf.Success(added.message);
                    return RedirectToAction("Index", "Lib");
                }
                else
                {
                    _notyf.Error(added.message);
                    return RedirectToAction("AddBook", addEditBookDto);
                }
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View(addEditBookDto);
            }
        }

        #endregion


        #region Remove


        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public IActionResult LibRemoveBook(int bookId)
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
                return View("Index", "Lib");
            }
        }


        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public IActionResult LibRemoveUser(int UserId)
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
                return View("Index", "Lib");
            }
        }

        #endregion


        #region Edit User


        [Authorize(Roles = "Librarian")]
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
                return RedirectToAction("Index", "Lib");
            }
        }


        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public IActionResult EditUser(UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return View(userDto);
            }
            try
            {
                var updated = _admin.EditUserDataPost(userDto);
                if (updated)
                {
                    _notyf.Success("User updated successfully");
                    return RedirectToAction("Index", "Lib");
                }
                else
                {
                    _notyf.Error("Error in updating user... Please try again!");
                    return RedirectToAction("Index", "Lib");
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


        [Authorize(Roles = "Librarian")]
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
                _notyf.Error("Please try again.");
                return RedirectToAction("Index", "Lib");
            }
        }


        [Authorize(Roles = "Librarian")]
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
                    return RedirectToAction("Index", "Lib");
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


        [Authorize(Roles = "Librarian")]
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

        [Authorize(Roles = "Librarian")]
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
                    return RedirectToAction("Index", "Lib");
                }
                else
                {
                    _notyf.Error(assignBook.message ?? "An error occurred.");
                    return RedirectToAction("AssignBook", "Lib");
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


        [Authorize(Roles = "Librarian")]
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

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public IActionResult EditAssignBook(AssignedBooksDto assignedBooksDto)
        {
            try
            {
                var result = _admin.ReturnBookPost(assignedBooksDto);

                if (result.result = true)
                {
                    _notyf.Success(result.message);
                    return RedirectToAction("Index", "Lib");

                }
                else
                {
                    _notyf.Error(result.message);
                    return RedirectToAction("EditAssignBook", "Lib");

                }
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Lib");
            }
        }

        #endregion


        #region Assignbook by book


        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public IActionResult AssignBookByBook(int id)
        {
            try
            {
                var assignBookData = _lib.GetLibAssignBookData(id);
                return View(assignBookData);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Lib");
            }
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public IActionResult AssignBookByBook(AssignBookDto assignBookDto)
        {
            try
            {
                var loggedIn = "";
                if (User.Identity.IsAuthenticated)
                {
                    loggedIn = User.FindFirst("UserId")?.Value;
                }

                var result = _lib.LibAssignBookByBookPost(assignBookDto, loggedIn);

                if (result.result = true)
                {
                    _notyf.Success(result.message);
                    return RedirectToAction("Index", "Lib");

                }
                else
                {
                    _notyf.Error(result.message);
                    return RedirectToAction("EditAssignBook", "Lib");

                }
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View("Index", "Lib");
            }
        }

        #endregion

    }
}
