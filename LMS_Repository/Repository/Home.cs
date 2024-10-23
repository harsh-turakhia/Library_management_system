using LMS_Data_Entity.Dto;
using LMS_Repository.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Repository.Repository
{
    public class Home : IHome
    {
        private readonly string _connectionString;

        public Home(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public bool RegisterUser(RegisterDto registerDto)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand("CALL RegisterProcedure(@Name , @Email , @Password , @Address , @PhoneNumber)", connection))
                    {
                        command.Parameters.AddWithValue("@Name", registerDto.Name);
                        command.Parameters.AddWithValue("@Email", registerDto.Email);
                        command.Parameters.AddWithValue("@Password", registerDto.Password);
                        command.Parameters.AddWithValue("@Address", registerDto.Address);
                        command.Parameters.AddWithValue("@PhoneNumber", registerDto.PhoneNumber);

                        var result = command.ExecuteNonQuery();

                        return result != 0;

                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
