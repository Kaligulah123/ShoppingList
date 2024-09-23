namespace ShoppingList.MVVM.Models
{
    public class ShopList
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Code { get; set; }
        public bool IsShared { get; set; }
        public List<string> Users { get; set; } = [];

    }
}
