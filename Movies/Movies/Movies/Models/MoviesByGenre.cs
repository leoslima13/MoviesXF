using System.Collections.Generic;

namespace Movies.Models
{
    public class MoviesByGenre : List<Movie>
    {
        public MoviesByGenre(Genre genre, List<Movie> items) : base(items)
        {
            Genre = genre;
        }
        
        public Genre Genre { get; }
    }
}