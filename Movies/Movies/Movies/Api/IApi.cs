using System.Collections.Generic;
using System.Threading.Tasks;
using Movies.Models;
using Refit;

namespace Movies.Api
{
    public interface IApi
    {
        [Get(
            "/discover/movie?include_adult=false&include_video=false&language=en-US&page={pageId}&sort_by=popularity.desc")]
        Task<PageResult<Movie>> GetMovies(int pageId);
        
        [Get(
            "/discover/movie?include_adult=false&include_video=false&language=en-US&page={pageId}&sort_by=popularity.desc&with_genres={genreId}")]
        Task<PageResult<Movie>> GetMoviesWithGenre(int pageId, int genreId);

        [Get("/movie/popular?language=en-US&page={pageId}")]
        Task<PageResult<Movie>> GetPopularMovies(int pageId);

        [Get("/genre/movie/list")]
        Task<GenreResult> GetGenres();
    }
}