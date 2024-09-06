using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Introduction.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private const string connectionString = "Host=localhost:5432;" + "Username=postgres;" + "Password=postgres;" + "Database=postgres";

        [HttpPost]
        public ActionResult Post([FromBody] User user)
        {
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                string commandText = $"INSERT INTO \"User\" VALUES(@Id, @FirstName, @LastName, @DateOfBirth);";
                using var command = new NpgsqlCommand(commandText, connection);

                command.Parameters.AddWithValue("@Id", NpgsqlTypes.NpgsqlDbType.Uuid, Guid.NewGuid());
                command.Parameters.AddWithValue("@FirstName", user.FirstName); 
                command.Parameters.AddWithValue("@LastName", user.LastName);
                command.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);

                connection.Open();

                var numberOfCommits = command.ExecuteNonQuery();

                connection.Close();
                if (numberOfCommits == 0)
                    return BadRequest();
                return Ok("Adding successful...");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                string commandText = "DELETE FROM \"User\" WHERE \"Id\" = @Id;";
                using var command = new NpgsqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                var numberOfCommits = command.ExecuteNonQuery();

                connection.Close();
                if (numberOfCommits == 0)
                    return BadRequest();
                return Ok("Deleting successful...");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(Guid id, User user)
        {
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                string commandText = $"UPDATE  \"User\" SET FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth WHERE Id = @id";
                using var command = new NpgsqlCommand(commandText, connection);

                command.Parameters.AddWithValue("@Id", NpgsqlTypes.NpgsqlDbType.Uuid, Guid.NewGuid());
                command.Parameters.AddWithValue("@FirstName", user.FirstName);
                command.Parameters.AddWithValue("@LastName", user.LastName);
                command.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);

                connection.Open();

                var numberOfCommits = command.ExecuteNonQuery();

                connection.Close();
                if (numberOfCommits == 0)
                    return BadRequest();
                return Ok("Update successful...");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            try
            {
                var user = new User();
                using var connection = new NpgsqlConnection(connectionString);
                var commandText = "SELECT * FROM \"User\" WHERE \"Id\" = @id;";
                using var command = new NpgsqlCommand(commandText, connection);

                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                using NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();

                    user.Id = Guid.Parse(reader[0].ToString());
                    user.FirstName = reader[1].ToString();
                    user.LastName = reader["LastName"].ToString();
                    var temp = Convert.ToDateTime(reader[3]);
                    user.DateOfBirth = DateOnly.FromDateTime(temp);
                }

                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUserAndPosts/{id}")]
        public ActionResult GetUserAndPosts(Guid id)
        {
            try
            {
                var facebookPost = new FacebookPost();
                var user = new User();
                using var connection = new NpgsqlConnection(connectionString);
                var commandText = "SELECT * FROM \"User\" INNER JOIN \"FacebookPost\" ON \"User\".\"Id\" = \"FacebookPost\".\"UserId\" WHERE \"User\".\"Id\" = @id";
                using var command = new NpgsqlCommand(commandText, connection);

                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                using NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();

                    user.Id = Guid.Parse(reader[0].ToString());
                    user.FirstName = reader[1].ToString();
                    user.LastName = reader["LastName"].ToString();
                    var temp = Convert.ToDateTime(reader[3]);
                    user.DateOfBirth = DateOnly.FromDateTime(temp);

                    facebookPost.Id = Guid.Parse(reader[4].ToString());
                    facebookPost.Caption = reader[5].ToString();
                    facebookPost.UserId = Guid.Parse(reader[6].ToString());

                    facebookPost.User = user;
                }

                if (user == null || facebookPost == null)
                {
                    return NotFound();
                }
                return Ok(facebookPost);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
