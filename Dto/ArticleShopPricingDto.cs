namespace GroceryOptimizerApi.Dto
{
    public class ArticleShopPricingDto
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }         // Matches `articleId`
        public int ShopId { get; set; }            // Matches `shopId`
        public decimal PricePerUnit { get; set; }  // Matches `pricePerUnit`

        // Additional fields for details
        public string ShopName { get; set; }       // Matches `shopName`
        public int PriceUnitId { get; set; }
        public string PriceUnitName { get; set; }       // Matches `unitName`
        public string PriceUnitNameShort { get; set; }
        public DateTime DateInserted { get; set; }
    }
}
