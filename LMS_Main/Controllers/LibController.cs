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


        #region AddEditUser

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public IActionResult AddEditUser(int? id)
        {
            AddEditUserDto addEditUserDto;

            if (id.HasValue)
            {
                addEditUserDto = _admin.GetEditUserData(id.Value);
            }
            else
            {
                addEditUserDto = new AddEditUserDto();
            }

            return View("AddEditUser", addEditUserDto);
        }


        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public IActionResult AddEditUser(AddEditUserDto addEditUserDto)
        {
            if (!ModelState.IsValid)
            {
                return View("AddEditUser", addEditUserDto);
            }

            try
            {
                bool emailExists = _admin.CheckEmailExist(addEditUserDto.Email);

                if (emailExists && !addEditUserDto.UserId.HasValue)
                {
                    ModelState.AddModelError("Email", "Email already exists!");
                    return View("AddEditUser", addEditUserDto);
                }

                if (addEditUserDto.UserId.HasValue)
                {
                    var updated = _admin.EditUserDataPost(addEditUserDto);
                    _notyf.Success("User updated successfully");
                }
                else
                {
                    var loggedIn = "";
                    if (User.Identity.IsAuthenticated)
                    {
                        loggedIn = User.FindFirst("UserId")?.Value;
                    }

                    var added = _admin.AdminAddUserPost(addEditUserDto, loggedIn);
                    _notyf.Success("User added successfully");
                }

                return RedirectToAction("Index", "Lib");
            }
            catch
            {
                _notyf.Warning("An error occurred. Please try again.");
                return View("AddEditUser", addEditUserDto);
            }
        }


        #endregion


        #region AddEditBook

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public IActionResult AddEditBook(int? bookId)
        {
            try
            {
                var data = new AddEditBookDto();

                if (bookId.HasValue)
                {
                    data = _admin.GetEditBookData(bookId.Value);
                }
                else
                {
                    data = _admin.GetAddBookData();
                }

                return View(data);
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return RedirectToAction("Index", "Lib");
            }
        }


        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public IActionResult AddEditBook(AddEditBookDto addEditBookDto)
        {
            if (!ModelState.IsValid)
            {
                return View(addEditBookDto);
            }

            try
            {
                var loggedIn = User.Identity.IsAuthenticated ? User.FindFirst("UserId")?.Value : "";
                bool result;

                if (addEditBookDto.BookId == 0)
                {
                    result = _admin.AdminAddBookPost(addEditBookDto, loggedIn);
                    if (result)
                    {
                        _notyf.Success("Book added successfully!");
                    }
                    else
                    {
                        _notyf.Error("Error in adding the book!");
                    }
                }
                else
                {
                    result = _admin.AdminEditBookPost(addEditBookDto, loggedIn);
                    if (result)
                    {
                        _notyf.Success("Book updated successfully!");
                    }
                    else
                    {
                        _notyf.Error("Error in updating the book!");
                    }
                }

                return RedirectToAction("Index", "Lib");
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View(addEditBookDto);
            }
        }


        #endregion

    }
}
