﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketMicroService.Contracts;
using TicketMicroService.Models;
using TicketMicroService.Models.DataTransferObjects;

namespace TicketMicroService.Repositories
{
    public class TicketRepository : RepositoryBase<Ticket>, ITicketRepository
    {
        public TicketRepository(RepositoryDbContext context) : base(context)
        {
        }

        public void CreateTicket(Ticket ticket)
        {
            Create(ticket);
        }

        public void DeleteTicket(Ticket ticket)
        {
            Delete(ticket);
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsPaginationAsync(int pageIndex, int pageSize, 
            TicketModelForSearchDTO searchModel, bool trackChanges)
        {
            return await FindByCondition(ticket =>
                (string.IsNullOrWhiteSpace(searchModel.Email) || ticket.Email.Contains(searchModel.Email)) &&
                (searchModel.DateTime == new DateTime() || searchModel.DateTime == ticket.DateTime), trackChanges)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(ticket => ticket.Places)
                .ToListAsync();
        }

        public async Task<Ticket> GetTicketAsync(int ticketId, bool trackChanges)
        {
            return await FindByCondition(ticket => ticket.Id == ticketId, trackChanges)
                .Include(ticket => ticket.Places)
                .SingleOrDefaultAsync();
        }

        public async Task<Ticket> GetTicketByDateTimeAndEmail(DateTime dateTime, string email, bool trackChanges)
        {
            return await FindByCondition(ticket => ticket.DateTime == dateTime && ticket.Email == email, trackChanges)
                .Include(ticket => ticket.Places)
                .SingleOrDefaultAsync();
        }

        public async Task<int> GetTicketsCountAsync(TicketModelForSearchDTO searchModel, bool trackChanges)
        {
            return await FindByCondition(ticket =>
                (string.IsNullOrWhiteSpace(searchModel.Email) || ticket.Email.Contains(searchModel.Email)) &&
                (searchModel.DateTime == new DateTime() || searchModel.DateTime == ticket.DateTime), trackChanges)
                .CountAsync();
        }

        public async Task<bool> IsPlacesFree(DateTime dateTime, IEnumerable<Place> places, int ticketId)
        {
            var tickets = await FindByCondition(ticket =>
                ticket.DateTime == dateTime && ticket.Id != ticketId, false)
                .Include(ticket => ticket.Places)
                .ToListAsync();
            if (tickets.SelectMany(ticket =>
                            ticket.Places.SelectMany(place => places
                                .Where(aplace => aplace.Row == place.Row && aplace.Site == place.Site)
                                .Select(aplace => new { }))).Any())
                return false;
            return true;
        }

        public async Task<Ticket> IsEmailHasAlreadyTicketForThisTime(string email, DateTime dateTime)
        {
            var res = await FindByCondition(t => t.Email == email && t.DateTime == dateTime, false)
                .Include(x => x.Places)
                .SingleOrDefaultAsync();
            return res;
        }

        public async Task SaveAsync() => await context.SaveChangesAsync();
    }
}
