﻿using Microsoft.EntityFrameworkCore;
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
        Task<IEnumerable<MovieTag>> GetAllForSpecificMovie(Guid id);
        Tag GetOskarId();
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
            MovieTag movieTag = _cinemaContext.MovieTags.Find(id);
            var result = _cinemaContext.MovieTags.Remove(movieTag).Entity;
            return result;
        }

        public async Task<IEnumerable<MovieTag>> GetAll()
        {
            var data = await _cinemaContext.MovieTags.Include(m => m.Movie).Include(x => x.Tag).ToListAsync();
            return data;
        }

        public async Task<IEnumerable<MovieTag>> GetAllForSpecificMovie(Guid id)
        {
            var data = await _cinemaContext.MovieTags.Include(m => m.Movie).Where(m=>m.MovieId == id).ToListAsync();
            return data;
        }

        public Task<MovieTag> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }
        public Tag GetOskarId()
        {
            var oskarTag = _cinemaContext.Tags.SingleOrDefault(x => x.Name.Equals("oskar"));
            return oskarTag;
        }

        public MovieTag Insert(MovieTag obj)
        {

            var data = _cinemaContext.MovieTags.Add(obj).Entity;
            return data;
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
