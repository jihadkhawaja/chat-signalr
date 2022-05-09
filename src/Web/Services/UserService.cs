using Microsoft.EntityFrameworkCore;
using MobileChat.Web.Database;
using MobileChat.Web.Helpers;
using MobileChat.Web.Interfaces;
using MobileChat.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MobileChat.Web.Services
{
    public class UserService : IUser
    {
        private readonly DatabaseContext context;
        public UserService(DatabaseContext context)
        {
            this.context = context;
        }
        public Task<bool> Create(User entry)
        {
            try
            {
                entry.DateCreated = DateTime.UtcNow;
                context.Users.Add(entry);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<User> ReadById(ulong id)
        {
            return Task.FromResult(context.Users.Single(x => x.Id == id));
        }

        public Task<User> ReadByEmail(string email)
        {
            return Task.FromResult(context.Users.Single(x => x.Email == email));
        }

        public Task<User> ReadByUsername(string username)
        {
            return Task.FromResult(context.Users.Single(x => x.Username == username));
        }

        public Task<HashSet<User>> ReadAll()
        {
            return Task.FromResult(context.Users.ToHashSet());
        }

        public Task<bool> Update(User entry)
        {
            try
            {
                User dbentry = context.Users.Single(x => x.Id == entry.Id);

                if (dbentry is not null)
                {
                    context.Entry(dbentry).State = EntityState.Detached;
                    context.Users.Update(entry);

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

        public Task<bool> Delete(ulong id)
        {
            try
            {
                User entry = context.Users.Single(x => x.Id == id);

                if (entry is not null)
                {
                    context.Users.Remove(entry);
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

        public Task<bool> UserExist(string emailorusername)
        {
            try
            {
                bool IsEmail = Regex.IsMatch(emailorusername, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

                User dbentry;

                if (IsEmail)
                {
                    dbentry = context.Users.Single(x => x.Email == emailorusername);

                    if (dbentry is not null)
                    {
                        return Task.FromResult(true);
                    }
                }
                else
                {
                    dbentry = context.Users.Single(x => x.Username == emailorusername);

                    if (dbentry is not null)
                    {
                        return Task.FromResult(true);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return Task.FromResult(false);
        }
        public Task<bool> LogIn(string emailorusername, string password)
        {
            try
            {
                bool IsEmail = Regex.IsMatch(emailorusername, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

                User dbentry;

                if (IsEmail)
                {
                    dbentry = context.Users.Single(x => x.Email == emailorusername);
                }
                else
                {
                    dbentry = context.Users.Single(x => x.Username == emailorusername);
                }

                if (CryptographyHelper.ComparePassword(password, dbentry.Password))
                {
                    return Task.FromResult(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return Task.FromResult(false);
        }

        public Task<bool> LogOut(string emailorusername)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangePassword(string emailorusername, string oldpassword, string newpassword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddFriend(User user, User friend)
        {
            if(user == null || friend == null)
                return Task.FromResult(false);

            try
            {
                if(context.UsersFriends.SingleOrDefault(x => x.UserId == user.Id && x.FriendUserId == friend.Id) != null)
                    return Task.FromResult(false);

                //get friend id from username or email
                User friendUser = context.Users.SingleOrDefault(x => x.Username == friend.Username || x.Email == friend.Email);
                if(friendUser == null) return Task.FromResult(false);

                UserFriend entry = new() { UserId = user.Id, FriendUserId = friendUser.Id, DateCreated = DateTime.UtcNow };
                context.UsersFriends.Add(entry);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> RemoveFriend(User user, User friend)
        {
            if (user == null || friend == null)
                return Task.FromResult(false);

            try
            {
                UserFriend entry = context.UsersFriends.SingleOrDefault(x => x.UserId == user.Id && x.FriendUserId == friend.Id);
                if (entry is null)
                    return Task.FromResult(false);

                context.UsersFriends.Remove(entry);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> SendFriendRequest(User user, User friend)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AcceptFriendRequest(User user, User friend)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RejectFriendRequest(User user, User friend)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BlockFriend(User user, User friend)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UnblockFriend(User user, User friend)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUserFriends(User user)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUserFriendRequests(User user)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUserBlockedFriends(User user)
        {
            throw new NotImplementedException();
        }
    }
}
