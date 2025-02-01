using Dapper;
using GroceryOptimizerApi.Dto;
using GroceryOptimizerApi.Entities;
using GroceryOptimizerApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Security.Claims;

namespace GroceryOptimizerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ArticleController(IConfiguration configuration, ILogger<ArticleController> logger) : ControllerBase
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");
        private readonly ILogger<ArticleController> _logger = logger;

        [HttpGet("ApiTest")]
        public async Task<IActionResult> Test()
        {
            return Ok("API OK.");
        }

        [HttpGet("Articles")]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            try
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("PriceUnits")]
        public async Task<ActionResult<IEnumerable<PriceUnit>>> GetPriceUnits()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"select id, unitname, shortname from priceunit";
                    var priceUnits = await connection.QueryAsync<PriceUnit>(query);

                    return Ok(priceUnits);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ArticleDetails")]
        public async Task<ActionResult<Article>> GetArticle(int articleId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"SELECT 
	                article.id, 
	                userid as AddedByUserId,
	                priceunitid,
	                article.name, 
	                note,
	                u.""UserName"" as AddedByUserName,
	                priceunit.unitname as PriceUnitName,
                    priceunit.shortname as PriceUnitNameShort 
                FROM Article
                INNER JOIN ""AspNetUsers"" u ON u.""UserId"" = Article.userid
                INNER JOIN priceunit ON priceunit.id = Article.priceunitid
                WHERE article.id = @Id";
                    var article = await connection.QuerySingleOrDefaultAsync<ArticleDto>(query, new { Id = articleId });

                    return Ok(article);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("LatestArticlePricing")]
        public async Task<ActionResult<IEnumerable<ArticleShopPricing>>> GetLatestArticlePricings(int articleId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"
                    SELECT DISTINCT ON (asp.ArticleId, asp.ShopId) 
                        asp.""Id"", 
                        asp.ArticleId, 
                        asp.ShopId, 
                        asp.PricePerUnit,
                        shop.name AS ShopName,
                        article.priceunitid,
                        priceunit.unitname AS PriceUnitName,
                        priceunit.shortname AS PriceUnitNameShort,
                        asp.dateinserted
                    FROM ArticleShopPricing asp
                    INNER JOIN article ON article.id = asp.articleid
                    INNER JOIN shop ON shop.id = asp.shopid
                    INNER JOIN priceunit ON priceunit.id = article.priceunitid
                    WHERE asp.ArticleId = @ArticleId
                    ORDER BY asp.ArticleId, asp.ShopId, asp.dateinserted DESC;";

                    var pricings = await connection.QueryAsync<ArticleShopPricingDto>(query, new { ArticleId = articleId });

                    return Ok(pricings);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Create")]
        [Authorize]
        public async Task<ActionResult<int>> CreateArticle([FromBody] CreateArticleRequestModel request)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int userId = int.Parse(identity.FindFirst("UserId").Value);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var existingEntryQuery = @"SELECT COUNT(1) FROM article where LOWER(name) = LOWER(@Name)";
                    var existingRows = await connection.ExecuteScalarAsync<int>(existingEntryQuery, new { Name = request.Name });
                    if (existingRows != 0)
                        throw new Exception($"{request.Name} already exists");

                    var insertQuery = @"
                        INSERT INTO article (userid, name, priceunitid, note)
                        VALUES (@UserId, @Name, @PriceUnitId, @Note)
                        RETURNING id;";

                    var insertedId = await connection.ExecuteScalarAsync<int>(insertQuery, new
                    {
                        UserId = userId,
                        Name = request.Name,
                        PriceUnitId = request.PriceUnitId,
                        Note = request.Note
                    });

                    return Ok(insertedId);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddArticleShopPricing")]
        public async Task<ActionResult<string>> AddArticleShopPricing([FromBody] UpdatePricingRequestModel request)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var insertQuery = @"
                    INSERT INTO articleshoppricing (articleid, shopid, priceperunit, dateinserted)
                    VALUES (@ArticleId, @ShopId, @PricePerUnit, NOW());";

                    var affectedRows = await connection.ExecuteAsync(insertQuery, new
                    {
                        ArticleId = request.ArticleId,
                        ShopId = request.ShopId,
                        PricePerUnit = request.PricePerUnit
                    });

                    if (affectedRows == 0)
                        return BadRequest("Failed to add pricing.");

                    return Ok("Pricing added successfully.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UpdateArticleShopPricing")]
        public async Task<ActionResult<string>> UpdateArticleShopPricing([FromBody] UpdatePricingRequestModel request)
        {
            return BadRequest("Update action not supported.");
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var getQuery = @"SELECT 1 FROM ArticleShopPricing WHERE shopid = @ShopId";
                var existingRow = await connection.QuerySingleOrDefaultAsync<int?>(getQuery, new { ShopId = request.ShopId });
                if (existingRow is null)
                    return BadRequest("Update failed. Pricing doesnt exist for shop with id: " + request.ShopId);

                var query = @"
                    UPDATE ArticleShopPricing
                    SET PricePerUnit = @PricePerUnit
                    WHERE ShopId = @ShopId AND ArticleId = @ArticleId";

                var affectedRows = await connection.ExecuteAsync(query, new
                {
                    PricePerUnit = request.PricePerUnit,
                    ShopId = request.ShopId,
                    ArticleId = request.ArticleId
                });

                if (affectedRows == 0)
                    return BadRequest("Failed to update pricing.");

                return Ok("Pricing updated successfully.");
            }
        }
    }
}
