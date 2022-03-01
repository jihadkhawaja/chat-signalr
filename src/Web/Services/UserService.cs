﻿using MobileChatWeb.Database;
using MobileChatWeb.Helpers;
using MobileChatWeb.Interfaces;
using MobileChatWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MobileChatWeb.Services
{
    public class UserService : IUser
    {
        private readonly DatabaseContext context;
        public static User User { get; private set; }
        public UserService(DatabaseContext context)
        {
            this.context = context;
        }
        public DatabaseContext Context()
        {
            return context;
        }
        public Task<bool> Create(User entry)
        {
            try
            {
                entry.DateCreated = DateTime.UtcNow;
                User = entry;
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

        public Task<bool> UserExist(string username, string email)
        {
            try
            {
                User dbentry = context.Users.Single(x => x.Username == username || x.Email == email);

                if (dbentry is not null)
                {
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
                    User = dbentry;
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
        public User GetUser()
        {
            return User;
        }
    }
}