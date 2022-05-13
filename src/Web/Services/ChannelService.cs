﻿using Microsoft.EntityFrameworkCore;
using MobileChat.Web.Database;
using MobileChat.Web.Interfaces;
using MobileChat.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileChat.Web.Services
{
    public class ChannelService : IChannel
    {
        private readonly DatabaseContext context;
        public ChannelService(DatabaseContext context)
        {
            this.context = context;
        }

        public Task<bool> Create(Channel entry)
        {
            try
            {
                context.Channels.Add(entry);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<Channel> ReadById(Guid id)
        {
            return Task.FromResult(context.Channels.Single(x => x.Id == id));
        }

        public Task<bool> AddUsers(Guid userid, Guid channelid, params string[] usernames)
        {
            try
            {
                ChannelUser[] channelUsers = new ChannelUser[usernames.Length];

                if (!ContainUser(userid).Result)
                {
                    ChannelUser channelCreator = new ChannelUser
                    {
                        ChannelId = channelid,
                        UserId = userid,
                        DateCreated = DateTime.UtcNow
                    };

                    context.ChannelUsers.Add(channelCreator);
                }

                for (int i = 0; i < usernames.Length; i++)
                {
                    Guid currentuserid = context.Users.Single(x => x.Username == usernames[i]).Id;

                    if (ContainUser(currentuserid).Result) continue;

                    channelUsers[i] = new ChannelUser()
                    {
                        ChannelId = channelid,
                        UserId = currentuserid,
                        DateCreated = DateTime.UtcNow
                    };
                }

                context.ChannelUsers.AddRange(channelUsers);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<HashSet<User>> GetUsers(Guid channelid)
        {
            HashSet<User> channelUsers = new HashSet<User>();
            try
            {
                List<ChannelUser> users = context.ChannelUsers.Where(x => x.ChannelId == channelid).ToList();
                foreach (ChannelUser user in users)
                {
                    channelUsers.Add(context.Users.Single(x => x.Id == user.UserId));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Task.FromResult(channelUsers);
            }

            return Task.FromResult(channelUsers);
        }

        public Task<bool> ContainUser(Guid userid)
        {
            return Task.FromResult(context.ChannelUsers.Any(x => x.UserId == userid));
        }

        public Task<HashSet<Channel>> GetUserChannels(Guid userid)
        {
            HashSet<Channel> channels = new HashSet<Channel>();
            try
            {
                List<ChannelUser> users = context.ChannelUsers.Where(x => x.UserId == userid).ToList();
                foreach (ChannelUser user in users)
                {
                    channels.Add(context.Channels.Single(x => x.Id == user.ChannelId));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Task.FromResult(channels);
            }

            return Task.FromResult(channels);
        }

        public Task<HashSet<Message>> GetChannelMessages(Guid channelid)
        {
            return Task.FromResult(context.Messages.Where(x => x.ChannelId == channelid).ToHashSet());
        }
    }
}
