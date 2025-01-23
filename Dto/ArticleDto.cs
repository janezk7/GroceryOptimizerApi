namespace GroceryOptimizerApi.Dto
{
    public class ArticleDto
    {
        public int Id { get; set; }                // Matches `id`
        public int AddedByUserId { get; set; }     // Matches `addedByUserId`
        public int PriceUnitId { get; set; }            // Matches `unitId`
        public string Name { get; set; }            // Matches `name`
        public string? Note { get; set; }           // Matches `note`, nullable

        // Additional fields for details
        public string AddedByUserName { get; set; } // Matches `addedByUserName`
        public string PriceUnitName { get; set; }   // Matches `priceUnitName`
        public string PriceUnitNameShort{ get; set; }
    }
}
