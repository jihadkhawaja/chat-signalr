using Microsoft.EntityFrameworkCore;
using MobileChat.Web.Database;
using MobileChat.Web.Interfaces;
using MobileChat.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileChat.Web.Services
{
    public class MessageService : IMessage
    {
        private readonly DatabaseContext context;
        public MessageService(DatabaseContext context)
        {
            this.context = context;
        }
        public Task<bool> Create(Message entry)
        {
            try
            {
                entry.DateCreated = DateTime.UtcNow;
                context.Messages.Add(entry);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> Delete(Guid id)
        {
            try
            {
                Message entry = context.Messages.Single(x => x.Id == id);

                if (entry is not null)
                {
                    context.Messages.Remove(entry);
                    context.SaveChanges();

                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch (Exception e)
            {
                return Task.FromResult(false);
            }
        }

        public Task<HashSet<Message>> ReadAll(ulong userid)
        {
            return Task.FromResult(context.Messages.Where(x => x.UserId == userid).ToHashSet());
        }

        public Task<Message> ReadById(Guid id)
        {
            return Task.FromResult(context.Messages.Single(x => x.Id == id));
        }

        public Task<bool> Update(Message entry)
        {
            try
            {
                Message dbentry = context.Messages.Single(x => x.Id == entry.Id);

                if (dbentry is not null)
                {
                    context.Entry(dbentry).State = EntityState.Detached;
                    context.Messages.Update(entry);

                    context.SaveChanges();

                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Task.FromResult(false);
            }
        }
    }
}
