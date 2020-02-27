using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterWorkShop.Cinema.API.Models;
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
        [Authorize(Roles = "admin")]
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
        public async Task<ActionResult<IEnumerable<CreateTicketResultModel>>> CreateTicket([FromBody]CreateTicketModel createTicketModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TicketDomainModel ticketDomainModel = new TicketDomainModel
            {
                ProjectionId = createTicketModel.ProjectionId,
                UserId = createTicketModel.UserId
            };

            var listOFSeats = createTicketModel.seatModels;


            List<CreateTicketResultModel> createTicketResultModels = new List<CreateTicketResultModel>();
            CreateTicketResultModel createTicketResultModel;

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
        public async Task<ActionResult<PaymentResponseModel>> ConfirmPayment(TicketPaymentConfirm ticketResultModels)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<TicketDomainModel> listOfTicketsIds = new List<TicketDomainModel>();
            foreach (var item in ticketResultModels.listOfTickets)
            {
                TicketDomainModel ticketDomainModel = new TicketDomainModel
                {
                    Id = item.Id
                };
                listOfTicketsIds.Add(ticketDomainModel);
            }

            PaymentResponse result;

            try
            {
                result = await _ticketService.ConfirmPayment(listOfTicketsIds);
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

                return BadRequest(errorResponse);
            }

            return Ok(result);

        }
    }
}