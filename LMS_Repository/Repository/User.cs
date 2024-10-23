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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LMS_Repository.Repository
{
    public class User : IUser
    {
        private readonly string _connectionString;

        public User(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public UserHomePageDto GetHomePageData()
        {
            var data = new UserHomePageDto();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();


                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "SELECT * FROM UserHomePageData();";
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var book = new UserHomePageBooksDto
                                {
                                    BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    AuthorName = reader.GetString(reader.GetOrdinal("AuthorName")),
                                    Publication = reader.GetString(reader.GetOrdinal("PublicationName")),
                                    Copies = reader.GetInt32(reader.GetOrdinal("Copies")),
                                    Language = reader.GetString(reader.GetOrdinal("LanguageName")),
                                    Price = reader.GetInt32(reader.GetOrdinal("Price")),
                                    //Description = reader.GetString(reader.GetOrdinal("Description")),
                                    //Pages = reader.GetInt32(reader.GetOrdinal("Pages")),
                                    //ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                                };
                                data.UserHomePageBooksList.Add(book);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return data;
        }

        public List<UserHomePageBooksDto> UserBookSearchHandler(string query)
        {
            var booksList = new List<UserHomePageBooksDto>();

            try
            {
                if (query == null)
                {
                    using (var connection = new NpgsqlConnection(_connectionString))
                    {
                        connection.Open();

                        using (NpgsqlCommand command = new NpgsqlCommand())
                        {
                            command.CommandText = "SELECT * FROM getbooks()";
                            command.Connection = connection;
                            command.CommandType = CommandType.Text;

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var book = new UserHomePageBooksDto
                                    {
                                        BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                        Title = reader.GetString(reader.GetOrdinal("Title")),
                                        AuthorName = reader.GetString(reader.GetOrdinal("AuthorName")),
                                        Publication = reader.GetString(reader.GetOrdinal("PublicationName")),
                                        Copies = reader.GetInt32(reader.GetOrdinal("Copies")),
                                        Language = reader.GetString(reader.GetOrdinal("LanguageName")),
                                        Price = reader.GetInt32(reader.GetOrdinal("Price")),
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
                            command.CommandText = "SELECT * FROM GetSearchedBooks(@query)";
                            command.Connection = connection;
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddWithValue("@query", query);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var book = new UserHomePageBooksDto
                                    {
                                        BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                        Title = reader.GetString(reader.GetOrdinal("Title")),
                                        AuthorName = reader.GetString(reader.GetOrdinal("AuthorName")),
                                        Publication = reader.GetString(reader.GetOrdinal("PublicationName")),
                                        Copies = reader.GetInt32(reader.GetOrdinal("Copies")),
                                        Language = reader.GetString(reader.GetOrdinal("LanguageName")),
                                        Price = reader.GetInt32(reader.GetOrdinal("Price")),
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

        public UserAssignedBooksDto GetMyBooks(string UserId)
        {
            var booksList = new UserAssignedBooksDto();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.CommandText = "SELECT * FROM userassignbooks(@UserId)";
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@UserId", UserId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var book = new AssignedBooksDto
                            {
                                AssignedId = reader.GetInt32(reader.GetOrdinal("AssignBookId")),
                                BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                BookName = reader.GetString(reader.GetOrdinal("Title")),
                                UserName = reader.GetString(reader.GetOrdinal("Name")),
                                StatusName = reader.GetString(reader.GetOrdinal("StatusName")),
                                IssuedDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("IssuedDate"))),
                                ReturnDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ReturnDate"))),
                                ReturnedOn = reader.IsDBNull(reader.GetOrdinal("ReturnedOn"))
                                                  ? (DateOnly?)null
                                                  : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ReturnedOn"))),
                            };
                            booksList.AssignedBooksList.Add(book);
                        }
                    }
                }
                return booksList;
            }
        }
    }
}
