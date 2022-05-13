﻿using Microsoft.EntityFrameworkCore;
using MobileChat.Web.Database;
using MobileChat.Web.Helpers;
using MobileChat.Web.Interfaces;
using MobileChat.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Task<User> ReadById(Guid id)
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

        public Task<bool> Delete(Guid id)
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
                User dbentry;

                if (PatternMatchHelper.IsEmail(emailorusername))
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
        public Task<bool> SignIn(string emailorusername, string password)
        {
            try
            {
                User dbentry;

                if (PatternMatchHelper.IsEmail(emailorusername))
                {
                    dbentry = context.Users.Single(x => x.Email == emailorusername);
                }
                else
                {
                    dbentry = context.Users.Single(x => x.Username == emailorusername);
                }

                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return Task.FromResult(false);
        }

        public Task<bool> SignOut(string emailorusername)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangePassword(string emailorusername, string oldpassword, string newpassword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddFriend(Guid userId, string friendEmailorusername)
        {
            if (string.IsNullOrEmpty(friendEmailorusername))
            {
                return Task.FromResult(false);
            }

            try
            {
                if (PatternMatchHelper.IsEmail(friendEmailorusername))
                {
                    //get user id from email
                    User user = context.Users.SingleOrDefault(x => x.Id == userId);
                    if (user == null)
                    {
                        return Task.FromResult(false);
                    }
                    //get friend id from email
                    User friendUser = context.Users.SingleOrDefault(x => x.Email == friendEmailorusername);
                    if (friendUser == null)
                    {
                        return Task.FromResult(false);
                    }

                    if (context.UsersFriends.SingleOrDefault(x => x.UserId == user.Id && x.FriendUserId == friendUser.Id) != null)
                    {
                        return Task.FromResult(false);
                    }

                    UserFriend entry = new() { UserId = user.Id, FriendUserId = friendUser.Id, DateCreated = DateTime.UtcNow };
                    context.UsersFriends.Add(entry);
                    context.SaveChanges();
                }
                else
                {
                    //get user id from username
                    User user = context.Users.SingleOrDefault(x => x.Id == userId);
                    if (user == null)
                    {
                        return Task.FromResult(false);
                    }
                    //get friend id from username
                    User friendUser = context.Users.SingleOrDefault(x => x.Username == friendEmailorusername);
                    if (friendUser == null)
                    {
                        return Task.FromResult(false);
                    }

                    if (context.UsersFriends.SingleOrDefault(x => x.UserId == user.Id && x.FriendUserId == friendUser.Id) != null)
                    {
                        return Task.FromResult(false);
                    }

                    UserFriend entry = new() { UserId = user.Id, FriendUserId = friendUser.Id, DateCreated = DateTime.UtcNow };
                    context.UsersFriends.Add(entry);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> RemoveFriend(Guid userId, string friendEmailorusername)
        {
            if (string.IsNullOrEmpty(friendEmailorusername))
            {
                return Task.FromResult(false);
            }

            try
            {
                if (PatternMatchHelper.IsEmail(friendEmailorusername))
                {
                    //get user id from email
                    User user = context.Users.SingleOrDefault(x => x.Id == userId);
                    if (user == null)
                    {
                        return Task.FromResult(false);
                    }
                    //get friend id from email
                    User friendUser = context.Users.SingleOrDefault(x => x.Email == friendEmailorusername);
                    if (friendUser == null)
                    {
                        return Task.FromResult(false);
                    }

                    UserFriend entry = context.UsersFriends.SingleOrDefault(x => x.UserId == user.Id && x.FriendUserId == friendUser.Id);
                    if (entry is null)
                    {
                        return Task.FromResult(false);
                    }

                    context.UsersFriends.Remove(entry);
                }
                else
                {
                    //get user id from username
                    User user = context.Users.SingleOrDefault(x => x.Id == userId);
                    if (user == null)
                    {
                        return Task.FromResult(false);
                    }
                    //get friend id from username
                    User friendUser = context.Users.SingleOrDefault(x => x.Username == friendEmailorusername);
                    if (friendUser == null)
                    {
                        return Task.FromResult(false);
                    }

                    UserFriend entry = context.UsersFriends.SingleOrDefault(x => x.UserId == user.Id && x.FriendUserId == friendUser.Id);
                    if (entry is null)
                    {
                        return Task.FromResult(false);
                    }

                    context.UsersFriends.Remove(entry);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<string> GetDisplayName(Guid userId)
        {
            try
            {
                User user = context.Users.SingleOrDefault(x => x.Id == userId);
                if (user == null)
                {
                    return Task.FromResult(string.Empty);
                }

                return Task.FromResult(user.DisplayName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Task.FromResult(string.Empty);
            }
        }
    }
}