using System.Collections.Generic;
using Newtonsoft.Json;

namespace Movies.Models
{
    public class Genre
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonIgnore] public bool IsPopular => Id == -1;
    }

    public class GenreResult
    {
        [JsonProperty("genres")]
        public List<Genre> Genres { get; set; }
    }
}