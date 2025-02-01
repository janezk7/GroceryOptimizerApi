using Dapper;
using GroceryOptimizerApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace GroceryOptimizerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ShopController(IConfiguration configuration) : ControllerBase
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

        [HttpGet("Shops")]
        public async Task<ActionResult<IEnumerable<Shop>>> GetShops()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT * FROM Shop";
                    var shops = await connection.QueryAsync<Shop>(query);

                    return Ok(shops);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Shop")]
        public async Task<ActionResult<Shop>> GetShop(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT * FROM Shop WHERE Id = @Id";
                    var shop = await connection.QueryAsync<Shop>(query, new { Id = id });

                    return Ok(shop);
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }
        }
    }
}
