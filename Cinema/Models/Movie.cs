using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public Category? Category { get; set; } = null!;

        public int Duration { get; set; }
        public DateTime ReleaseDate { get; set; }

        [Required, MaxLength(50), MinLength(3)]
        public string Language { get; set; } = string.Empty;
        [Required]
        public string Description {  get; set; } = string.Empty;

        public string? Poster { get; set; } 

        public string? Trailer { get; set; }

        [Required]
        public int? AgeRestriction { get; set; } 

        public string? Subtitle { get; set; }
        public double Rate { get; set; }

        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
        public ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
    }
}
