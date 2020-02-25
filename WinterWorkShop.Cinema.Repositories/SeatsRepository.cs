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
    public interface ISeatsRepository : IRepository<Seat> 
    {
        Task<IEnumerable<Seat>> GetAllOfSpecificProjection(object id);
        IEnumerable<Seat> GetAllOfSpecificAuditorium(object id); 
    }
    public class SeatsRepository : ISeatsRepository
    {
        private CinemaContext _cinemaContext;

        public SeatsRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public Seat Delete(object id)
        {
            Seat existing = _cinemaContext.Seats.Find(id);
            var result = _cinemaContext.Seats.Remove(existing).Entity;

            return result;
        }

        public async Task<IEnumerable<Seat>> GetAll()
        {
            var data = await _cinemaContext.Seats.ToListAsync();

            return data;
        }

        public async Task<IEnumerable<Seat>> GetAllOfSpecificProjection(object id)
        {

            var findProjection = await _cinemaContext.Projections.FindAsync(id);
            var seats = await _cinemaContext.Seats.Where(x => x.AuditoriumId.Equals(findProjection.AuditoriumId)).ToListAsync();

            return seats;
        }

        public async Task<Seat> GetByIdAsync(object id)
        {
            return await _cinemaContext.Seats.FindAsync(id);
        }

        public Seat Insert(Seat obj)
        {
            var data = _cinemaContext.Seats.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Seat Update(Seat obj)
        {
            var updatedEntry = _cinemaContext.Seats.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry;
        }

        public IEnumerable<Seat> GetAllOfSpecificAuditorium(object id)
        {
            var seats = _cinemaContext.Seats.Where(x => x.AuditoriumId.Equals((int)id)).ToList();

            return seats;

            //var findAuditorium = _cinemaContext.Auditoriums.Find(id);
            //if (findAuditorium != null)
            //{
            //    var seats = _cinemaContext.Seats.Where(x => x.AuditoriumId.Equals(findAuditorium.Id)).ToList();
            //    return seats; 
            //}
            //return null; 
        }

    }
}
