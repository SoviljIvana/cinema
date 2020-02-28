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
    public interface ITagRepository : IRepository<Tag> 
    {
        Task<IEnumerable<Tag>> GetAllAwords();
        Task<IEnumerable<Tag>> GetAllGenre();

        Task<IEnumerable<Tag>> GetAllActores();
        Task<IEnumerable<Tag>> GetAllCreators();
        Task<IEnumerable<Tag>> GetAllLanguage();
        Tag GetByIdName(string name);

    }
    public class TagRepository : ITagRepository
    {
        private CinemaContext _cinemaContext;

        public TagRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }
        public Tag Delete(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Tag>> GetAll()
        {
            var data = await _cinemaContext.Tags.ToListAsync();
            return data;
        }
        public async Task<IEnumerable<Tag>> GetAllGenre()
        {
            var data = await _cinemaContext.Tags.Where(x=>x.Type.Equals("genre")).ToListAsync();
            return data;
        }
        public async Task<IEnumerable<Tag>> GetAllActores()
        {
            var data = await _cinemaContext.Tags.Where(x => x.Type.Equals("actor")).ToListAsync();
            return data;
        }
        public async Task<IEnumerable<Tag>> GetAllCreators()
        {
            var data = await _cinemaContext.Tags.Where(x => x.Type.Equals("actor")).ToListAsync();

            return data;
        }
        public async Task<IEnumerable<Tag>> GetAllLanguage()
        {
            var data = await _cinemaContext.Tags.Where(x => x.Type.Equals("language")).ToListAsync();

            return data;
        }
        public async Task<IEnumerable<Tag>> GetAllStates()
        {
            var data = await _cinemaContext.Tags.Where(x => x.Type.Equals("state")).ToListAsync();

            return data;
        }
        public async Task<IEnumerable<Tag>> GetAllAwords()
        {
            var data = await _cinemaContext.Tags.Where(x => x.Type.Equals("aword")).ToListAsync();

            return data;
        }
        public Task<Tag> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }
        public  Tag GetByIdName(string name)
        {
            var data =  _cinemaContext.Tags.SingleOrDefault(x => x.Name.Equals(name));
            return data;
        }

        public Tag Insert(Tag obj)
        {
            return _cinemaContext.Tags.Add(obj).Entity;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public Tag Update(Tag obj)
        {
            throw new NotImplementedException();
        }
    }
}
