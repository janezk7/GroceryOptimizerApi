using Dapper;
using GroceryOptimizerApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Security.Claims;

namespace GroceryOptimizerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly string _connectionString;

        public ArticleController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Get user id from claims
                //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                var query = "SELECT Id, UserId, Name, Note FROM Article";
                var articles = await connection.QueryAsync<Article>(query);

                return Ok(articles);
            }
        }
    }
}
