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
    public interface IAuditoriumsRepository : IRepository<Auditorium> 
    {
        Task<IEnumerable<Auditorium>> GetByAuditName(string name, int id);
        IEnumerable<Auditorium> GetAllOfSpecificCinema(object id); 
    }
    public class AuditoriumsRepository : IAuditoriumsRepository
    {
        private CinemaContext _cinemaContext;

        public AuditoriumsRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }


        public async Task<IEnumerable<Auditorium>> GetByAuditName(string name, int id)
        {
            var data = await _cinemaContext.Auditoriums.Where(x => x.Name.Equals(name) && x.CinemaId.Equals(id)).ToListAsync();

            return data;
        }

        public Auditorium Delete(object id)
        {
            Auditorium existingAuditorium = _cinemaContext.Auditoriums.Find(id);
            //var seatsInAuditorium = _cinemaContext.Seats.Where(x=>x.AuditoriumId == (int)id).ToList();  
            //var seatsInAuditorium = _cinemaContext.Seats.Where(x => x.AuditoriumId.Equals(id));
            //existingAuditorium.Seats = seatsInAuditorium.ToList();

            //if (existingAuditorium.Seats != null)
            //{
            //    foreach (var seat in seatsInAuditorium)
            //    {
            //        _cinemaContext.Seats.Remove(seat);
            //        existingAuditorium.Seats.Remove(seat);
            //    }
            //    _cinemaContext.SaveChanges();

            //}
            var result = _cinemaContext.Auditoriums.Remove(existingAuditorium);

            _cinemaContext.SaveChanges();

            return result.Entity;
        }

        public async Task<IEnumerable<Auditorium>> GetAll()
        {
            var data = await _cinemaContext.Auditoriums.ToListAsync();

            return data;
        }

        public async Task<Auditorium> GetByIdAsync(object id)
        {
            return await _cinemaContext.Auditoriums.FindAsync(id);
        }

        public Auditorium Insert(Auditorium obj)
        {
            var data = _cinemaContext.Auditoriums.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Auditorium Update(Auditorium obj)
        {
            var updatedEntry = _cinemaContext.Auditoriums.Attach(obj);
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry.Entity;
        }
        public IEnumerable<Auditorium> GetAllOfSpecificCinema(object id)
        {

            var findCinema = _cinemaContext.Cinemas.Find(id);
            var auditoriums = _cinemaContext.Auditoriums.Where(x => x.CinemaId.Equals(findCinema.Id)).ToList();

            return auditoriums;
        }

    }
}
