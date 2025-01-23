namespace GroceryOptimizerApi.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PriceUnitId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
    }
}
