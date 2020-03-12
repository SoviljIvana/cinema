using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;


        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;

        }

        /// <summary>
        /// Returns all tickets
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "superUser, admin")]
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<TicketDomainModel>>> GetAsync()
        {
            IEnumerable<TicketDomainModel> ticketDomainModels;
            ticketDomainModels = await _ticketService.GetAllTickets();

            if (ticketDomainModels == null)
            {
                ticketDomainModels = new List<TicketDomainModel>();
            }

            return Ok(ticketDomainModels);
        }

        /// <summary>
        /// Adds a ticket
        /// </summary>
        /// <param name="createTicketModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "user, superUser, admin")]

        public async Task<ActionResult<IEnumerable<TicketResultModel>>> CreateTicket([FromBody]CreateTicketModel createTicketModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TicketDomainModel ticketDomainModel = new TicketDomainModel
            {
                ProjectionId = createTicketModel.ProjectionId,
                UserName = createTicketModel.UserName
            };

            var listOFSeats = createTicketModel.seatModels;


            List<TicketResultModel> createTicketResultModels = new List<TicketResultModel>();
            TicketResultModel createTicketResultModel;

            foreach (var item in listOFSeats)
            {
                try
                {
                    ticketDomainModel.SeatId = item.Id;
                    createTicketResultModel = await _ticketService.CreateNewTicket(ticketDomainModel);
                }
                catch (DbUpdateException e)
                {

                    ErrorResponseModel errorResponse = new ErrorResponseModel
                    {
                        ErrorMessage = e.InnerException.Message ?? e.Message,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };

                    return BadRequest(errorResponse);
                }

                if (!createTicketResultModel.IsSuccessful)
                {
                    ErrorResponseModel errorResponse = new ErrorResponseModel()
                    {
                        ErrorMessage = createTicketResultModel.ErrorMessage,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };

                    return BadRequest(errorResponse);
                }

                createTicketResultModels.Add(createTicketResultModel);
            }

            return createTicketResultModels;
        }

        [HttpPost]
        [Route("payValue")]
        [Authorize(Roles = "user, superUser, admin")]

        public async Task<ActionResult<PaymentResponse>> ConfirmPayment(TicketPaymentConfirm ticketPaymentConfirm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PaymentResponse result;

            try
            {
                if (ticketPaymentConfirm.PaymentSuccess)
                {
                    result = await _ticketService.ConfirmPayment(ticketPaymentConfirm.UserName);
                }
                else
                {
                    result = await _ticketService.DeleteTicketsPaymentUnsuccessful(ticketPaymentConfirm.UserName);
                }
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                    {
                        ErrorMessage = e.InnerException.Message ?? e.Message,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                return BadRequest(errorResponse);
            }

            if (!result.IsSuccess)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = result.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return NotFound(errorResponse);
            }

            return Ok(result);
        }

        [Authorize(Roles = "user, superUser, admin")]
        [HttpGet]
        [Route("allTickets/{username}")]
        public async Task<ActionResult<IEnumerable<TicketDomainModel>>> GetAllUnpaidTicketsForUser(string username)
        {
            IEnumerable<TicketDomainModel> ticketDomainModels;
            ticketDomainModels = await _ticketService.GetAllTicketsForThisUser(username);

            if (ticketDomainModels == null)
            {
                NotFound(Messages.TICKET_NOT_FOUND);
            }

            return Ok(ticketDomainModels);
        }

        [Authorize(Roles = "user, superUser, admin")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            TicketResultModel deletedTicket;
            try
            {
                deletedTicket = await _ticketService.DeleteTicketById(id);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }
            if(deletedTicket == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.TICKET_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }
            if(deletedTicket.Ticket == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.TICKET_NOT_FOUND,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }
            return Accepted("tickets//" + deletedTicket.Ticket.Id, deletedTicket);

        }
    }
}