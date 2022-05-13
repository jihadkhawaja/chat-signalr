using MobileChat.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileChat.Web.Interfaces
{
    public interface IUser
    {
        //crud
        Task<bool> Create(User user);
        Task<User> ReadById(Guid id);
        Task<User> ReadByEmail(string email);
        Task<User> ReadByUsername(string username);
        Task<HashSet<User>> ReadAll();
        Task<bool> Update(User user);
        Task<bool> Delete(Guid id);
        //custom
        Task<bool> UserExist(string emailorusername);
        Task<bool> SignIn(string emailorusername, string password);
        Task<bool> SignOut(string emailorusername);
        Task<bool> ChangePassword(string emailorusername, string oldpassword, string newpassword);
        Task<bool> AddFriend(Guid userId, string friendEmailorusername);
        Task<bool> RemoveFriend(Guid userId, string friendEmailorusername);
        Task<string> GetDisplayName(Guid userId);
    }
}
