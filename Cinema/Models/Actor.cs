using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required, MaxLength(100),MinLength(3)]
        public string Name { get; set; } = string.Empty;

        public int? Age { get; set; }

        [Required, MaxLength(10)]
        public string Gender { get; set; } = string.Empty;

        public string? Image { get; set; }

        [MaxLength(500)]
        public string? Biography { get; set; }

        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }
}
