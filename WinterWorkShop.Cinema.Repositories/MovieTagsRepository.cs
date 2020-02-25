using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IMovieTagsRepository : IRepository<MovieTag>
    {

    }

    public class MovieTagsRepository : IMovieTagsRepository
    {
        private CinemaContext _cinemaContext;

        public MovieTagsRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public MovieTag Delete(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MovieTag>> GetAll()
        {
            var data = await _cinemaContext.MovieTags.Include(m => m.Movie).Include(x => x.Tag).ToListAsync();
            return data;

        }

        public Task<MovieTag> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public MovieTag Insert(MovieTag obj)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public MovieTag Update(MovieTag obj)
        {
            throw new NotImplementedException();
        }
    }
}
