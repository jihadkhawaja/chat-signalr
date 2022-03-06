using MobileChat.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileChat.Web.Interfaces
{
    public interface IMessage
    {
        //crud
        Task<bool> Create(Message entry);
        Task<Message> ReadById(Guid id);
        Task<HashSet<Message>> ReadAll(ulong userid);
        Task<bool> Update(Message entry);
        Task<bool> Delete(Guid id);
    }
}
