using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using LootLogger.Commands;
using PcapDotNet.Core;
using PcapDotNet.Packets;

namespace LootLogger
{
    public class AresLootBot {
        private const string Token = "NjgwMDcwOTExNjM0MDQ2OTk2.XlUV9Q.3K6OKU0r1lqoOmHOzRpdYoeUQnI";
        static DiscordClient discord;
        static CommandsNextModule commands;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Token,
                TokenType = DSharpPlus.TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

 
            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!",
                EnableDms = false,
                EnableMentionPrefix = false,
               
            });
            commands.RegisterCommands<TransmitLogs>();
       
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }

    
}

