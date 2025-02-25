﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketMicroService.Models;
using TicketMicroService.Models.DataTransferObjects;

namespace TicketMicroService.Contracts
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllTicketsPaginationAsync(int pageIndex, int pageSize, TicketModelForSearchDTO searchModel, bool trackChanges);
        Task<int> GetTicketsCountAsync(TicketModelForSearchDTO searchModel, bool trackChanges);
        Task<Ticket> GetTicketAsync(int ticketId, bool trackChanges);
        Task<Ticket> GetTicketByDateTimeAndEmail(DateTime dateTime, string email, bool trackChanges);
        Task<bool> IsPlacesFree(DateTime dateTime, IEnumerable<Place> places, int ticketId);
        Task<Ticket> IsEmailHasAlreadyTicketForThisTime(string email, DateTime dateTime);
        void CreateTicket(Ticket ticket);
        void DeleteTicket(Ticket ticket);
        Task SaveAsync();
    }
}
