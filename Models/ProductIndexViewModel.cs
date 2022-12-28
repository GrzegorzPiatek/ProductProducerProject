namespace PWProject.Models
{
    public class ProductIndexViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public AlcoholType SelectedAlcoholType { get; set; }
    }

}
