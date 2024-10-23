using AspNetCoreHero.ToastNotification.Abstractions;
using LMS_Data_Entity.Dto;
using LMS_Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Repository.Repository
{
    public class Auth : IAuth
    {
        private readonly string _connectionString;

        public Auth(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }



        public UserDto authenticateUser(string email , string password)
        {
            UserDto user = null;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
               

                using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM AuthenticateUserFunc(@email, @password)", connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new UserDto
                            {
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                            };
                        }
                    }
                }
            }
            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}
