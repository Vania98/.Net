namespace Store_1.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int SellerId { get; set; }
    }
}
//Product клас модель для зберігання інформації про певний продукт у магазині.
//Він містить властивості для зберігання різних деталей, таких як ідентифікатор, ім’я, ціна, опис та ідентифікатор продавця.
