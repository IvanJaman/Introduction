using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Introduction.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FacebookPostController : ControllerBase
    {
        private const string connectionString = "Host=localhost:5432;" + "Username=postgres;" + "Password=postgres;" + "Database=postgres";

        [HttpPost]
        public ActionResult Post([FromBody] FacebookPost facebookPost)
        {
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                string commandText = $"INSERT INTO \"FacebookPost\" VALUES(@Id, @Caption, @UserId);";
                using var command = new NpgsqlCommand(commandText, connection);

                command.Parameters.AddWithValue("@Id", NpgsqlTypes.NpgsqlDbType.Uuid, Guid.NewGuid());
                command.Parameters.AddWithValue("@Caption", facebookPost.Caption);
                command.Parameters.AddWithValue("@UserId", facebookPost.UserId);

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
                string commandText = "DELETE FROM \"FacebookPost\" WHERE \"Id\" = @Id;";
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
        public ActionResult Put(Guid id, FacebookPost facebookPost)
        {
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                string commandText = $"UPDATE  \"FacebookPost\" SET Caption = @Caption, UserId = @UserId WHERE Id = @id";
                using var command = new NpgsqlCommand(commandText, connection);

                command.Parameters.AddWithValue("@Id", NpgsqlTypes.NpgsqlDbType.Uuid, Guid.NewGuid());

                connection.Open();

                var numberOfCommits = command.ExecuteNonQuery();

                connection.Close();
                if (numberOfCommits == 0)
                    return BadRequest();
                return Ok("Update successful...");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            try
            {
                var facebookPost = new FacebookPost();
                using var connection = new NpgsqlConnection(connectionString);
                var commandText = "SELECT * FROM \"FacebookPost\" WHERE \"Id\" = @id;";
                using var command = new NpgsqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();

                    facebookPost.Id = Guid.Parse(reader[0].ToString());
                    facebookPost.Caption = reader[1].ToString();
                    facebookPost.UserId = Guid.Parse(reader[2].ToString());

                    reader.Close();
                }

                if (facebookPost == null)
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
