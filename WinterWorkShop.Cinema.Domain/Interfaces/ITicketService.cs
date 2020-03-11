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
        Task<TicketResultModel> CreateNewTicket(TicketDomainModel ticketDomainModel);
        Task<PaymentResponse> ConfirmPayment(string username);
        Task<TicketDomainModel> DeleteTicket(Guid id);
        Task<TicketDomainModel> DeleteTicketFromProjection(Guid id);
        Task<IEnumerable<TicketDomainModel>> GetAllTicketsForThisUser(string username);
        Task<PaymentResponse> DeleteTicketsPaymentUnsuccessful(string username);
        Task<TicketResultModel> DeleteTicketById(Guid id);

    }
}
