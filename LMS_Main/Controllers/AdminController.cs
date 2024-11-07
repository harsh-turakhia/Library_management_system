using AspNetCoreHero.ToastNotification.Abstractions;
using Library_management_system.Models;
using LMS_Data_Entity.Dto;
using LMS_Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
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
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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


        #region AddEditUser

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult AddEditUser(AddEditUserDto addEditUserDto)
        {
            if (!ModelState.IsValid)
            {
                return View("AddEditUser", addEditUserDto);
            }

            try
            {
                bool emailExists = _admin.CheckEmailExist(addEditUserDto.Email);

                if (emailExists && !addEditUserDto.UserId.HasValue) // If editing, allow the same email
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

                return RedirectToAction("Index", "Admin");
            }
            catch
            {
                _notyf.Warning("An error occurred. Please try again.");
                return View("AddEditUser", addEditUserDto);
            }
        }


        #endregion


        #region AddEditBook

        [Authorize(Roles = "Admin")]
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
                return RedirectToAction("Index", "Admin");
            }
        }


        [Authorize(Roles = "Admin")]
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

                return RedirectToAction("Index", "Admin");
            }
            catch
            {
                _notyf.Warning("Please try again..!");
                return View(addEditBookDto);
            }
        }


        #endregion


        #region ExportData

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ExportData(int id)
        {
            try
            {
                if (id == 0)
                {
                    _notyf.Error("Something went wrong, Please try again..!");
                    return RedirectToAction("Index", "Admin");
                }
                
                else if (id == 1)
                {
                    var data = _admin.GetUsers();

                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Users");
                        worksheet.Cells[1, 1].Value = "Name";
                        worksheet.Cells[1, 2].Value = "Email";
                        worksheet.Cells[1, 3].Value = "Phone Number";
                        worksheet.Cells[1, 4].Value = "Address";
                        worksheet.Cells[1, 5].Value = "Role";

                        for (int i = 0; i < data.Count; i++)
                        {
                            worksheet.Cells[i + 2, 1].Value = data[i].Name;
                            worksheet.Cells[i + 2, 2].Value = data[i].Email;
                            worksheet.Cells[i + 2, 3].Value = data[i].PhoneNumber;
                            worksheet.Cells[i + 2, 4].Value = data[i].Address;
                            worksheet.Cells[i + 2, 5].Value = data[i].RoleName;
                        }

                        var stream = new MemoryStream();
                        package.SaveAs(stream);
                        var content = stream.ToArray();
                        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        var fileName = "UsersList.xlsx";

                        return File(content, contentType, fileName);
                    }
                }
                
                else if (id == 2)
                {
                    var data = _admin.GetBooks();

                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Books");
                        worksheet.Cells[1, 1].Value = "Title";
                        worksheet.Cells[1, 2].Value = "Copies";
                        worksheet.Cells[1, 3].Value = "Price";
                        worksheet.Cells[1, 4].Value = "Author Name";
                        worksheet.Cells[1, 5].Value = "Publication Name";
                        worksheet.Cells[1, 6].Value = "Language Name";

                        for (int i = 0; i < data.Count; i++)
                        {
                            worksheet.Cells[i + 2, 1].Value = data[i].Title;
                            worksheet.Cells[i + 2, 2].Value = data[i].Copies;
                            worksheet.Cells[i + 2, 3].Value = data[i].Price;
                            worksheet.Cells[i + 2, 4].Value = data[i].AuthorName;
                            worksheet.Cells[i + 2, 5].Value = data[i].PublicationName;
                            worksheet.Cells[i + 2, 6].Value = data[i].LanguageName;
                        }

                        var stream = new MemoryStream();
                        package.SaveAs(stream);
                        var content = stream.ToArray();
                        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        var fileName = "BooksList.xlsx";

                        return File(content, contentType, fileName);
                    }
                }
                
                else
                {
                    var data = _admin.GetAssignedBooks();

                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Assigned Books");
                        worksheet.Cells[1, 1].Value = "BookName";
                        worksheet.Cells[1, 2].Value = "UserName";
                        worksheet.Cells[1, 3].Value = "IssuedDate";
                        worksheet.Cells[1, 4].Value = "ReturnDate";
                        worksheet.Cells[1, 5].Value = "Status";

                        for (int i = 0; i < data.Count; i++)
                        {
                            worksheet.Cells[i + 2, 1].Value = data[i].BookName;
                            worksheet.Cells[i + 2, 2].Value = data[i].UserName;
                            worksheet.Cells[i + 2, 3].Value = data[i].IssuedDate;
                            worksheet.Cells[i + 2, 4].Value = data[i].ReturnDate;
                            worksheet.Cells[i + 2, 5].Value = data[i].StatusName;
                        }

                        var stream = new MemoryStream();
                        package.SaveAs(stream);
                        var content = stream.ToArray();
                        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        var fileName = "AssignedBooksList.xlsx";

                        return File(content, contentType, fileName);
                    }
                }
            }
            catch
            {
                _notyf.Error("Please try again..!");
                return RedirectToAction("Index", "Admin");
            }
        }

        #endregion
    }
}