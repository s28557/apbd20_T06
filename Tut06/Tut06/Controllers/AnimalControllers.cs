using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tut06.Models;
using Tut06.Models.DTOs;

namespace Tut06.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalControllers : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AnimalControllers(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetAnimals()
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM Animal";

        var reader =  command.ExecuteReader();
        
        List<Animal> animals = new List<Animal>();
        int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
        int nameOrdinal = reader.GetOrdinal("Name");

        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                IdAnimal = reader.GetInt32(idAnimalOrdinal),
                Name = reader.GetString(nameOrdinal)
            });
        }
        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal(AddAnimal addAnimal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = $"INSERT INTO Animal VALUES(@animalName, '', '', '')";
        command.Parameters.AddWithValue("@animalName", addAnimal.Name);

        command.ExecuteNonQuery();

        return Created();
    }
    
    [HttpDelete("{idAnimal}")]
    public IActionResult DeleteAnimal(int idAnimal)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "DELETE FROM Animal WHERE IdAnimal = @idAnimal";
            command.Parameters.AddWithValue("@idAnimal", idAnimal);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
    }

}