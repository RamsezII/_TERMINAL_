﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _TERMINAL_
{
    public sealed class Shell : Process
    {
        public interface IUser
        {
            public IEnumerable<string> Commands { get; }
            public void OnCmdLine(in string arg0, in LineParser line);
        }

        public static Shell instance = new();
        readonly HashSet<IUser> users = new();
        readonly Dictionary<string, IUser> commandOwners = new(StringComparer.OrdinalIgnoreCase);
        string[] commands;

        //----------------------------------------------------------------------------------------------------------

        public Shell() : base("~")
        {
            instance = this;
            userName = Directory.GetCurrentDirectory();
            RefreshPrefixe();
        }

        //----------------------------------------------------------------------------------------------------------

        void RefreshCommands()
        {
            commands = (
                from cmd in commandOwners.Keys
                orderby cmd
                select cmd
                ).ToArray();
        }

        public void AddUser(in IUser user)
        {
            users.Add(user);
            foreach (string cmd in user.Commands)
                commandOwners[cmd] = user;
            RefreshCommands();
        }

        public void RemoveUser(in IUser user)
        {
            users.Remove(user);
            foreach (string cmd in user.Commands)
                commandOwners.Remove(cmd);
            RefreshCommands();
        }

        public override void OnCmdLine(in string arg0, in LineParser line)
        {
            if (line.isCpl)
                line.OnCpls(arg0, commands);
            else if (commandOwners.TryGetValue(arg0, out IUser user))
                user.OnCmdLine(arg0, line);
            else
                base.OnCmdLine(arg0, line);
        }
    }
}