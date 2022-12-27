using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace PWProject.Models
{
    [Index(nameof(Name), IsUnique = true)]

    public class Producer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public List<Product>? Products { get; set; } = new();

    }
}
