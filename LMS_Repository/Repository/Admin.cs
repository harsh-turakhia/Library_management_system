using Library_management_system.Models;
using LMS_Data_Entity.Dto;
using LMS_Repository.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace LMS_Repository.Repository
{
    public class Admin : IAdmin
    {
        private readonly string _connectionString;

        public Admin(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public AdminHomePageDto GetAdminPageData(int currentPage = 1)
        {

            var adminPageData = new AdminHomePageDto();
            int pageSize = 3;

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    // Counts for pagination
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM PaginationCount()", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                adminPageData.UserCountForPagi = reader.GetInt32(reader.GetOrdinal("usercountforpagi"));
                                adminPageData.BookCountForPagi = reader.GetInt32(reader.GetOrdinal("bookcountforpagi"));
                                adminPageData.AssignedBookCountForPagi = reader.GetInt32(reader.GetOrdinal("assignedbookcountforpagi"));
                            }
                        }
                    }


                    // Get Total Books
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        //int pageSize = 4;
                        command.CommandText = "SELECT * FROM AdminGetBooks()";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        //command.Parameters.AddWithValue("PageSize", pageSize);
                        //command.Parameters.AddWithValue("Offsets", (currentPage - 1) * pageSize);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var book = new BooksDto
                                {
                                    BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    PublicationName = reader.GetString(reader.GetOrdinal("PublicationName")),
                                    AuthorName = reader.GetString(reader.GetOrdinal("AuthorName")),
                                    LanguageName = reader.GetString(reader.GetOrdinal("LanguageName")),
                                    Copies = reader.GetInt32(reader.GetOrdinal("Copies")),
                                    Price = reader.GetInt32(reader.GetOrdinal("Price")),
                                };

                                adminPageData.BooksList.Add(book);
                            }
                            //adminPageData.TotalPages = (int)Math.Ceiling((double)adminPageData.BookCountForPagi / pageSize);
                            //adminPageData.CurrentPage = currentPage;
                        }
                    }


                    // Get Assigned Books
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "SELECT * FROM GetAssignedBooks()";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var assignedBook = new AssignedBooksDto
                                {
                                    AssignedId = reader.GetInt32(reader.GetOrdinal("AssignBookId")),
                                    BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                    BookName = reader.GetString(reader.GetOrdinal("Title")),
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    UserName = reader.GetString(reader.GetOrdinal("Name")),
                                    //LibId = reader.GetInt32(reader.GetOrdinal("LibId")),
                                    //LibName = reader.GetString(reader.GetOrdinal("LibName")),
                                    ReturnDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ReturnDate"))),
                                    //ReturnedOn = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ReturnedOn"))),
                                    ReturnedOn = reader.IsDBNull(reader.GetOrdinal("ReturnedOn"))
                                                  ? (DateOnly?)null
                                                  : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ReturnedOn"))),

                                    IssuedDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("IssuedDate"))),
                                    Status = reader.GetInt32(reader.GetOrdinal("StatusId")),
                                    StatusName = reader.GetString(reader.GetOrdinal("StatusName")),

                                };

                                adminPageData.AssignedBooksList.Add(assignedBook);
                            }
                        }
                    }


                    // Get User
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "SELECT * FROM AdminGetUsers(@PageSize, @Offsets);";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("PageSize", pageSize);
                        command.Parameters.AddWithValue("Offsets", (currentPage - 1) * pageSize);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var user = new UserDto
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    RoleId = reader.GetInt32(reader.GetOrdinal("Role")),
                                    RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                };
                                adminPageData.UserList.Add(user);
                            }
                            
                        }
                    }
                    adminPageData.TotalPages = (int)Math.Ceiling((double)adminPageData.UserCountForPagi / pageSize);
                    adminPageData.CurrentPage = currentPage;
                                     

                    // User Count
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM AdminCountUser()", connection))
                    {
                        adminPageData.AllUserCount = Convert.ToInt32(command.ExecuteScalar());
                    }

                    // Books Count
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM AdminCountBooks();", connection))
                    {
                        adminPageData.AllBooksCount = Convert.ToInt32(command.ExecuteScalar());
                    }

                    // Publication Count
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM AdminCountPublication();", connection))
                    {
                        adminPageData.AllPublicationCount = Convert.ToInt32(command.ExecuteScalar());
                    }

                    // Author Count
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM AdminCountAuthor();", connection))
                    {
                        adminPageData.AllAuthorCount = Convert.ToInt32(command.ExecuteScalar());
                    }

                    // Assign Book Count
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM AdminCountAssignBook();", connection))
                    {
                        adminPageData.AllAssignedBookCount = Convert.ToInt32(command.ExecuteScalar());
                    }

                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return adminPageData;
        }


        public List<UserDto> AdminUserSearchHandler(string query)
        {
            var usersList = new List<UserDto>();

            try
            {
                if (query == null)
                {
                    using (var connection = new NpgsqlConnection(_connectionString))
                    {
                        connection.Open();

                        using (NpgsqlCommand command = new NpgsqlCommand())
                        {
                            command.CommandText = "SELECT * FROM AdminGetUsers()";
                            command.Connection = connection;
                            command.CommandType = CommandType.Text;

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var user = new UserDto
                                    {
                                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                        Name = reader.GetString(reader.GetOrdinal("Name")),
                                        Address = reader.GetString(reader.GetOrdinal("Address")),
                                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                        Email = reader.GetString(reader.GetOrdinal("Email")),
                                        RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                    };
                                    usersList.Add(user);
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (var connection = new NpgsqlConnection(_connectionString))
                    {
                        connection.Open();

                        using (NpgsqlCommand command = new NpgsqlCommand())
                        {
                            command.CommandText = "SELECT * FROM AdminGetSearchedUsers(@query)";
                            command.Connection = connection;
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddWithValue("@query", query);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var user = new UserDto
                                    {
                                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                        Name = reader.GetString(reader.GetOrdinal("Name")),
                                        Address = reader.GetString(reader.GetOrdinal("Address")),
                                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                        Email = reader.GetString(reader.GetOrdinal("Email")),
                                        RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                    };
                                    usersList.Add(user);
                                }
                            }
                        }
                    }

                }
                return usersList;
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }


        public List<BooksDto> AdminBookSearchHandler(string query)
        {
            var booksList = new List<BooksDto>();

            try
            {
                if (query == null)
                {
                    using (var connection = new NpgsqlConnection(_connectionString))
                    {
                        connection.Open();

                        using (NpgsqlCommand command = new NpgsqlCommand())
                        {
                            command.CommandText = "SELECT * FROM AdminGetBooks()";
                            command.Connection = connection;
                            command.CommandType = CommandType.Text;

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var book = new BooksDto
                                    {
                                        BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                        Title = reader.GetString(reader.GetOrdinal("Title")),
                                        AuthorName = reader.GetString(reader.GetOrdinal("AuthorName")),
                                        PublicationName = reader.GetString(reader.GetOrdinal("PublicationName")),
                                        LanguageName = reader.GetString(reader.GetOrdinal("LanguageName")),
                                        Price = reader.GetInt32(reader.GetOrdinal("Price")),
                                        Copies = reader.GetInt32(reader.GetOrdinal("Copies")),
                                    };
                                    booksList.Add(book);
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (var connection = new NpgsqlConnection(_connectionString))
                    {
                        connection.Open();

                        using (NpgsqlCommand command = new NpgsqlCommand())
                        {
                            command.CommandText = "SELECT * FROM AdminGetSearchedBooks(@query)";
                            command.Connection = connection;
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddWithValue("@query", query);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var book = new BooksDto
                                    {
                                        BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                        Title = reader.GetString(reader.GetOrdinal("Title")),
                                        AuthorName = reader.GetString(reader.GetOrdinal("AuthorName")),
                                        PublicationName = reader.GetString(reader.GetOrdinal("PublicationName")),
                                        LanguageName = reader.GetString(reader.GetOrdinal("LanguageName")),
                                        Price = reader.GetInt32(reader.GetOrdinal("Price")),
                                        Copies = reader.GetInt32(reader.GetOrdinal("Copies")),
                                    };
                                    booksList.Add(book);
                                }
                            }
                        }
                    }
                }
                return booksList;
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }


        public List<AssignedBooksDto> AdminAssignBookSearchHandler(string query)
        {
            var assignBooksList = new List<AssignedBooksDto>();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "SELECT * FROM GetSearchAssignedBooks(@query)";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@query", query);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var assignedBook = new AssignedBooksDto
                                {
                                    AssignedId = reader.GetInt32(reader.GetOrdinal("AssignBookId")),
                                    BookName = reader.GetString(reader.GetOrdinal("Title")),
                                    UserName = reader.GetString(reader.GetOrdinal("Name")),
                                    //LibId = reader.GetInt32(reader.GetOrdinal("LibId")),
                                    //LibName = reader.GetString(reader.GetOrdinal("LibName")),
                                    ReturnDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ReturnDate"))),
                                    //ReturnedOn = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ReturnedOn"))),

                                    ReturnedOn = reader.IsDBNull(reader.GetOrdinal("ReturnedOn"))
                                                  ? (DateOnly?)null
                                                  : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ReturnedOn"))),


                                    IssuedDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("IssuedDate"))),
                                    StatusName = reader.GetString(reader.GetOrdinal("StatusName")),
                                };

                                assignBooksList.Add(assignedBook);
                            }
                        }
                    }
                }


                return assignBooksList;
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public RegisterDto GetAddUserData()
        {
            var roleList = new RegisterDto();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "SELECT * FROM GetRoles()";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var role = new Role
                                {
                                    RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                                    RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                };
                                roleList.RoleList.Add(role);
                            }
                        }
                    }
                }
                return roleList;
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public bool CheckEmailExist(string email)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();


                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "SELECT EmailExistFunc(@Email)";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@email", email);

                        var result = command.ExecuteScalar();
                        return Convert.ToBoolean(result);
                    }
                }

            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public bool AdminAddUserPost(RegisterDto registerDto, string loggedIn)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "CALL AddUserProcedure(@Name , @Email , @Password , @PhoneNumber , @Address , @Role, @CreatedBy)";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        command.Parameters.AddWithValue("@Name", registerDto.Name);
                        command.Parameters.AddWithValue("@Email", registerDto.Email);
                        command.Parameters.AddWithValue("@Password", registerDto.Password);
                        command.Parameters.AddWithValue("@PhoneNumber", registerDto.PhoneNumber);
                        command.Parameters.AddWithValue("@Address", registerDto.Address);
                        command.Parameters.AddWithValue("@Role", registerDto.Role);
                        command.Parameters.AddWithValue("@CreatedBy", loggedIn);

                        var result = command.ExecuteNonQuery();

                        if (result != 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public AddEditBookDto GetAddBookData()
        {
            var data = new AddEditBookDto();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    // Publication list
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "SELECT * FROM getpublication()";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var pub = new Publications
                                {
                                    PublicationId = reader.GetInt32(reader.GetOrdinal("PublicationId")),
                                    PublicationName = reader.GetString(reader.GetOrdinal("PublicationName")),
                                };
                                data.PublicationsList.Add(pub);
                            }
                        }
                    }

                    // Author list
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "SELECT * FROM getauthors()";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var author = new Authors
                                {
                                    AuthorId = reader.GetInt32(reader.GetOrdinal("AuthorId")),
                                    AuthorName = reader.GetString(reader.GetOrdinal("AuthorName")),
                                };
                                data.AuthorsList.Add(author);
                            }
                        }
                    }

                    // Language list
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "SELECT * FROM getlanguage()";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var lang = new Language
                                {
                                    LanguageId = reader.GetInt32(reader.GetOrdinal("LanguageId")),
                                    LanguageName = reader.GetString(reader.GetOrdinal("LanguageName")),
                                };
                                data.LanguageList.Add(lang);
                            }
                        }
                    }


                    return data;
                }
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }

        }

        public bool AdminAddBookPost(AddEditBookDto addEditBookDto, string loggedIn)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "CALL AddBookProcedure(@Title , @Copies , @Price , @Author , @Publication , @Language, @AddedBy)";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        command.Parameters.AddWithValue("@Title", addEditBookDto.Title);
                        command.Parameters.AddWithValue("@Copies", addEditBookDto.Copies);
                        command.Parameters.AddWithValue("@Price", addEditBookDto.Price);
                        command.Parameters.AddWithValue("@Author", addEditBookDto.AuthorId);
                        command.Parameters.AddWithValue("@Publication", addEditBookDto.PublicationId);
                        command.Parameters.AddWithValue("@Language", addEditBookDto.LanguageId);
                        command.Parameters.AddWithValue("@AddedBy", loggedIn);


                        var result = command.ExecuteNonQuery();

                        if (result != 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public bool AdminRemoveUser(int UserId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "CALL AdminRemoveUser(@UserId)";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@UserId", UserId);

                        var result = command.ExecuteNonQuery();

                        if (result == -1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public bool AdminRemoveBook(int bookId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "CALL AdminRemoveBook(@bookId)";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@bookId", bookId);

                        var result = command.ExecuteNonQuery();

                        if (result == -1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public List<Publications> AddPublicationPost(string publicationName)
        {
            var publicationList = new List<Publications>();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "CALL addpublication(@publicationName)";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@publicationName", publicationName);

                        var result = command.ExecuteNonQuery();

                        if (result != 1)
                        {
                            using (var command2 = new NpgsqlCommand("SELECT * FROM GetPublication();", connection))
                            {
                                using (var reader = command2.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        var data = new Publications
                                        {
                                            PublicationId = reader.GetInt32(reader.GetOrdinal("PublicationId")),
                                            PublicationName = reader.GetString(reader.GetOrdinal("PublicationName")),
                                        };
                                        publicationList.Add(data);
                                    }
                                }
                            }
                            return publicationList;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public List<Language> AddLanguagePost(string languageName)
        {

            var languageList = new List<Language>();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "CALL addlanguage(@languageName)";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@languageName", languageName);

                        var result = command.ExecuteNonQuery();

                        if (result != 1)
                        {
                            using (var command2 = new NpgsqlCommand("SELECT * FROM getlanguage();", connection))
                            {
                                using (var reader = command2.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        var data = new Language
                                        {
                                            LanguageId = reader.GetInt32(reader.GetOrdinal("LanguageId")),
                                            LanguageName = reader.GetString(reader.GetOrdinal("LanguageName")),
                                        };
                                        languageList.Add(data);
                                    }
                                }
                            }
                            return languageList;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public List<Authors> AddAuthorPost(string authorName)
        {

            var authorsList = new List<Authors>();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "CALL addauthor(@authorName)";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@authorName", authorName);

                        var result = command.ExecuteNonQuery();

                        if (result != 1)
                        {
                            using (var command2 = new NpgsqlCommand("SELECT * FROM GetAuthors();", connection))
                            {
                                using (var reader = command2.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        var data = new Authors
                                        {
                                            AuthorId = reader.GetInt32(reader.GetOrdinal("AuthorId")),
                                            AuthorName = reader.GetString(reader.GetOrdinal("AuthorName")),
                                        };
                                        authorsList.Add(data);
                                    }
                                }
                            }
                            return authorsList;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public UserDto GetEditUserData(int id)
        {
            var userData = new UserDto();
            var UserId = id;

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand())
                    {
                        command.CommandText = "SELECT * FROM AdminGetUserById(@UserId);";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@UserId", UserId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                userData = new UserDto
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                    RoleId = reader.GetInt32(reader.GetOrdinal("Role")),
                                };
                            }
                        }

                        command.CommandText = "SELECT * FROM getroles();";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var role = new Role
                                {
                                    RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                                    RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                };
                                userData.RoleList.Add(role);
                            }
                        }
                    }
                    return userData;
                }
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public bool EditUserDataPost(UserDto userDto)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "CALL edituserprocedure(@UserId , @Name , @Email, @Address ,@PhoneNumber, @Role)";

                        command.Parameters.AddWithValue("@UserId", userDto.UserId);
                        command.Parameters.AddWithValue("@Name", userDto.Name);
                        command.Parameters.AddWithValue("@Email", userDto.Email);
                        command.Parameters.AddWithValue("@Address", userDto.Address);
                        command.Parameters.AddWithValue("@PhoneNumber", userDto.PhoneNumber);
                        command.Parameters.AddWithValue("@Role", userDto.RoleId);

                        var result = command.ExecuteNonQuery();

                        if (result != 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                }
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public AddEditBookDto GetEditBookData(int id)
        {
            var bookData = new AddEditBookDto();
            var BookId = id;
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand())
                    {
                        command.CommandText = "SELECT * FROM AdminGetBookById(@BookId);";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@BookId", BookId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bookData = new AddEditBookDto
                                {
                                    BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Copies = reader.GetInt32(reader.GetOrdinal("Copies")),
                                    Price = reader.GetInt32(reader.GetOrdinal("Price")),
                                    AuthorId = reader.GetInt32(reader.GetOrdinal("AuthorId")),
                                    AuthorName = reader.GetString(reader.GetOrdinal("AuthorName")),
                                    LanguageId = reader.GetInt32(reader.GetOrdinal("LanguageId")),
                                    LanguageName = reader.GetString(reader.GetOrdinal("LanguageName")),
                                    PublicationId = reader.GetInt32(reader.GetOrdinal("PublicationId")),
                                    PublicationName = reader.GetString(reader.GetOrdinal("PublicationName")),
                                };
                            }
                        }

                        // Publication list
                        command.CommandText = "SELECT * FROM getpublication();";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var pub = new Publications
                                {
                                    PublicationId = reader.GetInt32(reader.GetOrdinal("PublicationId")),
                                    PublicationName = reader.GetString(reader.GetOrdinal("PublicationName")),
                                };
                                bookData.PublicationsList.Add(pub);
                            }
                        }

                        // Language list
                        command.CommandText = "SELECT * FROM getlanguage();";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var lang = new Language
                                {
                                    LanguageId = reader.GetInt32(reader.GetOrdinal("LanguageId")),
                                    LanguageName = reader.GetString(reader.GetOrdinal("LanguageName")),
                                };
                                bookData.LanguageList.Add(lang);
                            }
                        }

                        // Authors list
                        command.CommandText = "SELECT * FROM getauthors();";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var author = new Authors
                                {
                                    AuthorId = reader.GetInt32(reader.GetOrdinal("AuthorId")),
                                    AuthorName = reader.GetString(reader.GetOrdinal("AuthorName")),
                                };
                                bookData.AuthorsList.Add(author);
                            }
                        }
                    }
                    return bookData;
                }

            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public bool EditBookDataPost(AddEditBookDto addEditBookDto)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "CALL editbookprocedure(@BookId , @Title , @Copies ,@Price, @PublicationId , @LanguageId ,  @AuthorId )";

                        command.Parameters.AddWithValue("@BookId", addEditBookDto.BookId);
                        command.Parameters.AddWithValue("@Title", addEditBookDto.Title);
                        command.Parameters.AddWithValue("@Copies", addEditBookDto.Copies);
                        command.Parameters.AddWithValue("@Price", addEditBookDto.Price);
                        command.Parameters.AddWithValue("@PublicationId", addEditBookDto.PublicationId);
                        command.Parameters.AddWithValue("@LanguageId", addEditBookDto.LanguageId);
                        command.Parameters.AddWithValue("@AuthorId", addEditBookDto.AuthorId);

                        var result = command.ExecuteNonQuery();

                        if (result != 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public AssignBookDto GetAssignBookData()
        {
            var assignBookData = new AssignBookDto();
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand())
                    {
                        // Users list
                        command.CommandText = "SELECT * FROM admingetusersforassign();";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var user = new UserDto
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                };
                                assignBookData.UserList.Add(user);
                            }
                        }


                        // Books list
                        command.CommandText = "SELECT * FROM admingetavailablebooks();";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var book = new BooksDto
                                {
                                    BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                };
                                assignBookData.BooksList.Add(book);
                            }
                        }

                        // Status list
                        command.CommandText = "SELECT * FROM getstatus();";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var stat = new Status
                                {
                                    StatusId = reader.GetInt32(reader.GetOrdinal("StatusId")),
                                    StatusName = reader.GetString(reader.GetOrdinal("StatusName")),
                                };
                                assignBookData.StatusList.Add(stat);
                            }
                        }
                        return assignBookData;
                    }
                }
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public (bool result, string message) AdminAssignBookPost(AssignBookDto assignBookDto, string loggedIn)
        {
            try
            {
                if (assignBookDto.ReturnDate > assignBookDto.IssuedDate)
                {
                    using (var connection = new NpgsqlConnection(_connectionString))
                    {
                        connection.Open();

                        using (var command = new NpgsqlCommand())
                        {

                            command.Connection = connection;
                            command.CommandText = "CALL assignbookprocedure(@BookId , @UserId , @IssuedDate ,@ReturnDate, @StatusId , @IssuedBy)";

                            command.Parameters.AddWithValue("@BookId", assignBookDto.BookId);
                            command.Parameters.AddWithValue("@UserId", assignBookDto.UserId);
                            command.Parameters.AddWithValue("@IssuedDate", assignBookDto.IssuedDate);
                            command.Parameters.AddWithValue("@ReturnDate", assignBookDto.ReturnDate);
                            command.Parameters.AddWithValue("@StatusId", assignBookDto.StatusId);
                            command.Parameters.AddWithValue("@IssuedBy", loggedIn);

                            var result = command.ExecuteNonQuery();

                            if (result != 1)
                            {
                                return (true, "Book assigned successfully");
                            }
                            else
                            {
                                return (false, "Failed to assign the book");
                            }
                        }
                    }
                }
                else
                {
                    return (false, "Return date should be greater than issue date");
                }

            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return (false, "Database error occurred");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return (false, "An error occurred");
            }
        }

        public AssignedBooksDto GetAssignedBookData(int id)
        {
            var assignBookData = new AssignedBooksDto();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand())
                    {
                        command.CommandText = "SELECT * FROM assignedbookdetails(@id);";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                assignBookData = new AssignedBooksDto
                                {
                                    AssignedId = reader.GetInt32(reader.GetOrdinal("AssignBookId")),
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    UserName = reader.GetString(reader.GetOrdinal("Name")),
                                    BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                    BookName = reader.GetString(reader.GetOrdinal("Title")),
                                    Status = reader.GetInt32(reader.GetOrdinal("Status")),
                                    ReturnDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ReturnDate"))),
                                    ReturnedOn = reader.IsDBNull(reader.GetOrdinal("ReturnedOn"))
                                                  ? (DateOnly?)null
                                                  : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ReturnedOn"))),
                                    IssuedDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("IssuedDate"))),
                                };
                            }
                        }

                        // Status list
                        command.CommandText = "SELECT * FROM getstatus();";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var status = new Status
                                {
                                    StatusId = reader.GetInt32(reader.GetOrdinal("StatusId")),
                                    StatusName = reader.GetString(reader.GetOrdinal("StatusName")),
                                };
                                assignBookData.StatusList.Add(status);
                            }
                        }
                        return assignBookData;
                    }
                }
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public (bool result, string message) ReturnBookPost(AssignedBooksDto assignedBooksDto)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "CALL returnbookprocedure(@AssignBookId , @Status , @ReturnedOn)";
                        var dateOnly = DateOnly.FromDateTime(DateTime.Now);

                        command.Parameters.AddWithValue("@AssignBookId", assignedBooksDto.AssignedId);
                        command.Parameters.AddWithValue("@Status", assignedBooksDto.Status);
                        command.Parameters.AddWithValue("@ReturnedOn", dateOnly);

                        var result = command.ExecuteNonQuery();

                        if (result != 1)
                        {
                            return (true, "Book returned successfully");
                        }
                        else
                        {
                            return (false, "Failed to return the book");
                        }
                    }
                }
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return (false, "Database error , try again!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return (false, "Exception caught , try again!");
            }
        }

    }
}

