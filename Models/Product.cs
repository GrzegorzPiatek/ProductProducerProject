using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PWProject.Models
{
    [Index(nameof(Name), IsUnique = true)]

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProducerId { get; set; }
        public virtual Producer? Producer { get; set; }
        public AlcoholType AlcoholType { get; set; }
    }
}
