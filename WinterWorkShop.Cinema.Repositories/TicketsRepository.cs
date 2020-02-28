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
    public interface ITicketRepository : IRepository<Ticket>
    {
        IEnumerable<Ticket> GetAllForSpecificProjection(Guid id);
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
            Ticket ticket = _cinemaContext.Tickets.Find(id);
            var result = _cinemaContext.Tickets.Remove(ticket).Entity;
            return result;
        }

        public async Task<IEnumerable<Ticket>> GetAll()
        {
            var data = await _cinemaContext.Tickets.Include(x=>x.Seat.Auditorium.Cinema).Include(x=>x.Projection.Movie).Include(x=>x.User).ToListAsync();
            return data;
        }

        public IEnumerable<Ticket> GetAllForSpecificProjection(Guid id)
        {
            var data = _cinemaContext.Tickets.Include(x => x.Seat).Include(x => x.Projection).Where(x=>x.ProjectionId == id).ToList();
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
