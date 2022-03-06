using MobileChat.Web.Database;
using MobileChat.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileChat.Web.Interfaces
{
    public interface IUser
    {
        //crud
        Task<bool> Create(User user);
        Task<User> ReadById(ulong id);
        Task<User> ReadByEmail(string email);
        Task<User> ReadByUsername(string username);
        Task<HashSet<User>> ReadAll();
        Task<bool> Update(User user);
        Task<bool> Delete(ulong id);
        //custom
        Task<bool> UserExist(string username, string email);
        Task<bool> LogIn(string emailorusername, string password);
    }
}
