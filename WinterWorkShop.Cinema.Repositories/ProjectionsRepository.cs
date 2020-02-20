﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IProjectionsRepository : IRepository<Projection> 
    {
        IEnumerable<Projection> GetByAuditoriumId(int salaId);
        Task<IEnumerable<Projection>> GetAllFromOneMovie(object id);
        Task<IEnumerable<Projection>> FilterAllProjections(string searchData);
        Task<IEnumerable<Projection>> FilterProjectionsByMovieTitle(string serachData);
        Task<IEnumerable<Projection>> FilterProjectionsByAuditoriumName(string serachData);
        Task<IEnumerable<Projection>> FilterProjectionsByCinemaName(string serachData);
        Task<IEnumerable<Projection>> FilterProjectionsByDates(DateTime startDate, DateTime endDate);


    }

    public class ProjectionsRepository : IProjectionsRepository
    {
        private CinemaContext _cinemaContext;

        public ProjectionsRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public Projection Delete(object id)
        {
            Projection existing = _cinemaContext.Projections.Find(id);
            var result = _cinemaContext.Projections.Remove(existing).Entity;

            return result;
        }

        public async Task<IEnumerable<Projection>> GetAll()
        {
            var data = await _cinemaContext.Projections.Include(x => x.Movie).Include(x => x.Auditorium).ToListAsync();
            
            return data;           
        }

        public async Task<IEnumerable<Projection>> GetAllFromOneMovie(object id)
        {
            var data = await _cinemaContext.Projections.Include(x => x.Movie).Include(x => x.Auditorium).ToListAsync();
            var data2 = data.Where(x => x.MovieId.Equals(id)).ToList();
            return data2;
        }


        public async Task<Projection> GetByIdAsync(object id)
        {
            return await _cinemaContext.Projections.FindAsync(id);
        }

        public IEnumerable<Projection> GetByAuditoriumId(int auditoriumId)
        {
            var projectionsData = _cinemaContext.Projections.Where(x => x.AuditoriumId == auditoriumId);

            return projectionsData;
        }

        public Projection Insert(Projection obj)
        {
            var data = _cinemaContext.Projections.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Projection Update(Projection obj)
        {
            var updatedEntry = _cinemaContext.Projections.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry;
        }
        /*///////////////////////////////////////////////////////
        FILTER PROJECTIONS
        //////////////////////////////////////////////////////*/
        public async Task<IEnumerable<Projection>> FilterAllProjections(string searchData)
        {
            var data = await _cinemaContext.Projections.Include(x => x.Movie).Include(x => x.Auditorium).Include(x => x.Auditorium.Cinema).
                            Where(y => y.Movie.Title.Contains(searchData) || y.Auditorium.Name.Contains(searchData) 
                            || y.Auditorium.Cinema.Name.Contains(searchData)
                            ).ToListAsync();
            return data;
        }
        public async Task<IEnumerable<Projection>> FilterProjectionsByMovieTitle(string searchData)
        {
            var data = await _cinemaContext.Projections.Include(x => x.Movie).Include(x => x.Auditorium).Include(x => x.Auditorium.Cinema).
                                        Where(y => y.Movie.Title.Contains(searchData)).ToListAsync(); 
            return data; 
        }
        public async Task<IEnumerable<Projection>> FilterProjectionsByAuditoriumName(string searchData)
        {
            var data = await _cinemaContext.Projections.Include(x => x.Movie).Include(x => x.Auditorium).Include(x => x.Auditorium.Cinema).
                                        Where(y => y.Auditorium.Name.Contains(searchData)).ToListAsync();
            return data;
        }
        public async Task<IEnumerable<Projection>> FilterProjectionsByCinemaName(string searchData)
        {
            var data = await _cinemaContext.Projections.Include(x => x.Movie).Include(x => x.Auditorium).Include(x => x.Auditorium.Cinema).
                                        Where(y => y.Auditorium.Cinema.Name.Contains(searchData)).ToListAsync();
            return data;
        }
        public async Task<IEnumerable<Projection>> FilterProjectionsByDates(DateTime startDate, DateTime endDate)
        {
            var data = await _cinemaContext.Projections.Include(x => x.Movie).Include(x => x.Auditorium).Include(x => x.Auditorium.Cinema).
                                        Where(y => y.DateTime >= startDate && y.DateTime<=endDate).ToListAsync();
            return data;
        }

    }
}
