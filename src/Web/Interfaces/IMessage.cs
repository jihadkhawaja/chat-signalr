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
        Task<HashSet<Message>> ReadAll();
        Task<HashSet<Message>> ReadAll(ulong userid);
        Task<HashSet<Message>> ReadAll(ulong userid, ulong userid2);
        Task<bool> Update(Message entry);
        Task<bool> Delete(Guid id);
    }
}
