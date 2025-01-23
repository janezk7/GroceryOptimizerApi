namespace GroceryOptimizerApi.Models
{
    public class UpdatePricingRequestModel
    {
        public int ShopId { get; set; }
        public int ArticleId { get; set; }
        public decimal PricePerUnit { get; set; }
    }
}
