namespace GroceryOptimizerApi.Entities
{
    public class ArticleShopPricing
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }         // Matches `articleId`
        public int ShopId { get; set; }            // Matches `shopId`
        public decimal PricePerUnit { get; set; }  // Matches `pricePerUnit`
    }
}
