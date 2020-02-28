using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDomainModel>> GetAllTickets();
        Task<CreateTicketResultModel> CreateNewTicket(TicketDomainModel ticketDomainModel);
        Task<PaymentResponse> ConfirmPayment(List<TicketDomainModel> ticketDomainModels);
        Task<TicketDomainModel> DeleteTicket(Guid id);

    }
}
