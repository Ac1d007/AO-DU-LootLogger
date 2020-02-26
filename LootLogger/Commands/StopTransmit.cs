using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootLogger.Commands
{
    class StopTransmit
    {
        [Command("stop")] // let's define this method as a command
        [Description("Command that stops the logs transmission")] // this will be displayed to tell users what this command does when they invoke help
        public async Task PostLogs(CommandContext ctx)
        {

            ctx.Channel.SendMessageAsync("Stopping loot log transmission");
            Environment.Exit(Environment.ExitCode);

        }
    }
}
