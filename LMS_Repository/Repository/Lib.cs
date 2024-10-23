using Library_management_system.Models;
using LMS_Data_Entity.Dto;
using LMS_Repository.Interface;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Repository.Repository
{
    public class Lib : ILib
    {
        private readonly string _connectionString;

        public Lib(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public LibHomePageDto GetLibPageData()
        {
            var libdata = new LibHomePageDto();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    // Get Total Books
                    using (var command = new NpgsqlCommand())
                    {
                        command.CommandText = "SELECT * FROM AdminGetBooks()";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var data = new BooksDto 
                                {
                                    BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    PublicationName = reader.GetString(reader.GetOrdinal("PublicationName")),
                                    AuthorName = reader.GetString(reader.GetOrdinal("AuthorName")),
                                    LanguageName = reader.GetString(reader.GetOrdinal("LanguageName")),
                                    Copies = reader.GetInt32(reader.GetOrdinal("Copies")),
                                    Price = reader.GetInt32(reader.GetOrdinal("Price")),
                                    RemainingCopies = reader.GetInt32(reader.GetOrdinal("totalremainingcopies"))
                                };
                                libdata.BooksList.Add(data);
                            }
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
                                libdata.AssignedBooksList.Add(assignedBook);
                            }
                        }
                    }

                    // Get User
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "SELECT * FROM LibGetUsers();";
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
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    RoleId = reader.GetInt32(reader.GetOrdinal("Role")),
                                    RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                                };
                                libdata.UserList.Add(user);
                            }
                        }
                    }

                    // User Count
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM LibCountUser()", connection))
                    {
                        libdata.TotalUserCount = Convert.ToInt32(command.ExecuteScalar());
                    }

                    // Books Count
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM AdminCountBooks();", connection))
                    {
                        libdata.TotalBooksCount = Convert.ToInt32(command.ExecuteScalar());
                    }

                    // Assign Book Count
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM AdminCountAssignBook();", connection))
                    {
                        libdata.TotalAssignedCopies = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
                return libdata;
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


        public List<UserDto> LibUserSearchHandler(string query)
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
                            command.CommandText = "SELECT * FROM LibGetUsers()";
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
                            command.CommandText = "SELECT * FROM LibGetSearchedUsers(@query)";
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


        public List<BooksDto> LibBookSearchHandler(string query)
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
                            command.CommandText = "SELECT * FROM LibGetSearchedBooks(@query)";
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


        public List<AssignedBooksDto> LibAssignBookSearchHandler(string query)
        {
            var assignBooksList = new List<AssignedBooksDto>();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    if (query == null)
                    {
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
                    else
                    {
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
                        command.CommandText = "SELECT * FROM LibGetRoles()";
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

        public bool LibAddUserPost(RegisterDto registerDto, string loggedIn)
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

        public (bool result, string message) LibAddBookPost(AddEditBookDto addEditBookDto, string loggedIn)
        {
            if (addEditBookDto.Copies < 1)
            {
                return (false, "Copies should ne greater than 1");
            }
            if (addEditBookDto.Price < 1)
            {
                return (false, "Price should ne greater than 1");
            }
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
                            return (true, "Book added successfully!");
                        }
                        else
                        {
                            return (false, "Fail to add book, try again!");
                        }
                    }
                }
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return (false, "Some Database error, try again!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return (false, "Exception caught");
            }
        }


        public AssignBookDto GetLibAssignBookData(int id)
        {
            var assignBookDetails = new AssignBookDto();

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
                                assignBookDetails.UserList.Add(user);
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
                                assignBookDetails.StatusList.Add(stat);
                            }
                        }

                        // Specific Book Details
                        command.CommandText = "SELECT * FROM admingetbookbyid(@id);";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                assignBookDetails.BookName = reader.GetString(reader.GetOrdinal("Title"));
                            }
                        }
                        assignBookDetails.BookId = id;
                    }

                    return assignBookDetails;
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

        public (bool result, string message) LibAssignBookByBookPost(AssignBookDto assignBookDto, string loggedIn)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand())
                    {
                        command.CommandText = "CALL assignbookprocedure(@BookId , @UserId , @IssuedDate , @ReturnDate , @Status , @IssuedBy)";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        command.Parameters.AddWithValue("@BookId", assignBookDto.BookId);
                        command.Parameters.AddWithValue("@UserId", assignBookDto.UserId);
                        command.Parameters.AddWithValue("@IssuedDate", assignBookDto.IssuedDate);
                        command.Parameters.AddWithValue("@ReturnDate", assignBookDto.ReturnDate);
                        command.Parameters.AddWithValue("@Status", assignBookDto.StatusId);
                        command.Parameters.AddWithValue("@IssuedBy", loggedIn);


                        var result = command.ExecuteNonQuery();

                        if (result != 1)
                        {
                            return (true, "Book assigned successfully!");
                        }
                        else
                        {
                            return (false, "Fail to assign book, try again!");
                        }
                    }
                }
            }
            catch (NpgsqlException dbEx)
            {
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return (false, "Some Database error, try again!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return (false, "Exception caught");
            }
        }
    }
}
