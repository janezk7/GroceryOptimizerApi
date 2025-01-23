namespace GroceryOptimizerApi.Entities
{
    public class Shop
    {
        public int Id { get; set; }          // Matches `id`
        public string Name { get; set; }     // Matches `name`
        public string? Note { get; set; }    // Matches `note`, nullable
    }
}
