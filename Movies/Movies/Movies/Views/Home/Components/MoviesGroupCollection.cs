using System.Collections.Generic;
using Movies.Helpers;
using Movies.Models;

namespace Movies.Views.Home.Components
{
    public class MoviesGroupCollection : GroupedCollection<Genre, MovieBindableItem>
    {
        public MoviesGroupCollection(Genre header, List<MovieBindableItem> items) : base(header, items)
        {
        }
    }
}