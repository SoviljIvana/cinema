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
        Task<IEnumerable<MovieTag>> GetAllMovieTagsForSpecificTag(string tag);
    }

    public class MovieTagRepository : IMovieTagsRepository
    {
        private CinemaContext _cinemaContext;

        public MovieTagRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public async Task<IEnumerable<MovieTag>> GetAllMovieTagsForSpecificTag(string tag)
        {
            var data = await _cinemaContext.MovieTags.Include(m=>m.Movie).Include(x => x.Tag).Where(y => y.Tag.Name.Contains(tag) || y.Tag.Type.Contains(tag)).ToListAsync();
            return data;
        }

        public MovieTag Delete(object id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MovieTag>> GetAll()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public MovieTag Update(MovieTag obj)
        {
            throw new NotImplementedException();
        }
    }
}
