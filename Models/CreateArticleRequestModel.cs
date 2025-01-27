using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace GroceryOptimizerApi.Models
{
    public class CreateArticleRequestModel
    {
        public string Name { get; set; }
        public int PriceUnitId { get; set; }
        public string Note {  get; set; }
    }
}
