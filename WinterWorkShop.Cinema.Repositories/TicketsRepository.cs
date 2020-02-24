﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface ITicketRepository : IRepository<Ticket>
    {

    }

    public class TicketsRepository : ITicketRepository
    {
        private CinemaContext _cinemaContext;
        public TicketsRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }
        public Ticket Delete(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Ticket>> GetAll()
        {
            var data = await _cinemaContext.Tickets.Include(x=>x.Seat.Auditorium.Cinema).Include(x=>x.Projection.Movie).Include(x=>x.User).ToListAsync();
            return data;
        }

        public async Task<Ticket> GetByIdAsync(object id)
        {
            var data = await _cinemaContext.Tickets.FindAsync(id);
            return data;
        }

        public Ticket Insert(Ticket obj)
        {
            var data = _cinemaContext.Tickets.Add(obj).Entity;
            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Ticket Update(Ticket obj)
        {
            var updateEntry = _cinemaContext.Tickets.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;
            return updateEntry;
        }
    }
}
