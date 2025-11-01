using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(100), MinLength(3)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }

}
