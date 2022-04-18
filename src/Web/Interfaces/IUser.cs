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
        Task<bool> UserExist(string emailorusername);
        Task<bool> LogIn(string emailorusername, string password);
        Task<bool> LogOut(string emailorusername);
        Task<bool> ChangePassword(string emailorusername, string oldpassword, string newpassword);
        Task<bool> AddFriend(User user, User friend);
        Task<bool> RemoveFriend(User user, User friend);
        Task<bool> SendFriendRequest(User user, User friend);
        Task<bool> AcceptFriendRequest(User user, User friend);
        Task<bool> RejectFriendRequest(User user, User friend);
        Task<bool> BlockFriend(User user, User friend);
        Task<bool> UnblockFriend(User user, User friend);
        Task<List<User>> GetUserFriends(User user);
        Task<List<User>> GetUserFriendRequests(User user);
        Task<List<User>> GetUserBlockedFriends(User user);
    }
}
