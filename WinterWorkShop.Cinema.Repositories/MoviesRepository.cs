using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IMoviesRepository : IRepository<Movie>
    {
        Task<IEnumerable<Movie>> GetCurrentAndNotCurrentMovies();
        Task<IEnumerable<Movie>> GetTopTenMovies();
        Task<IEnumerable<Movie>> GetCurrent();
        Task<IEnumerable<Movie>> GetAllWithMovieTags();
    }

    public class MoviesRepository : IMoviesRepository
    {
        private CinemaContext _cinemaContext;

        public MoviesRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public Movie Delete(object id)
        {
            Movie existing = _cinemaContext.Movies.Find(id);

            if (existing == null)
            {
                return null;
            }

            var result = _cinemaContext.Movies.Remove(existing);

            return result.Entity;
        }

        public async Task<IEnumerable<Movie>> GetAll()
        {
            return await _cinemaContext.Movies.ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetAllWithMovieTags()
        {
            var data = await _cinemaContext.Movies.Include(s => s.MovieTags).ToListAsync();
            return data;
        }

        public async Task<Movie> GetByIdAsync(object id)
        {
            var data = await _cinemaContext.Movies.FindAsync(id);

            return data;
        }

        public async Task<IEnumerable<Movie>> GetCurrent()
        {
            var data = await _cinemaContext.Movies
                .Where(x => x.Current).ToListAsync();

            return data;
        }


        public async Task<IEnumerable<Movie>> GetCurrentAndNotCurrentMovies()
        {
            var data = await _cinemaContext.Movies.ToListAsync();

            return data;
        }

        public Movie Insert(Movie obj)
        {
            var data = _cinemaContext.Movies.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Movie Update(Movie obj)
        {
            var updatedEntry = _cinemaContext.Movies.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry;
        }

        public async Task<IEnumerable<Movie>> GetTopTenMovies()
        {
            var data = await _cinemaContext.Movies.Include(x=>x.MovieTags).OrderByDescending(x => x.Rating).Take(11).ToListAsync();

            return data;
        }

    }
}
