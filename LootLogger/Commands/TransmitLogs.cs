using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LootLogger.Commands
{
    class TransmitLogs 
    {
        [Command("transmit")] // let's define this method as a command
        [Description("Command that starts the logs transmission")] // this will be displayed to tell users what this command does when they invoke help
        public async Task PostLogs(CommandContext ctx) {

            Commands.LootLogger logger = null;
            try
            {
                logger = new LootLogger(ctx);
                DateTime lastUploadDate = DateTime.MinValue;
                while (true){
                    if (lastUploadDate.AddMinutes(1) > DateTime.UtcNow)
                    {
                        return;
                    }
                    else
                    {
                       logger.GetLoots();
                    };
            }
                
                await Task.Delay(-1);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }

        }
    }
    public class LootLogger
    {

        private PacketHandler _eventHandler;
        private ILootService _lootService;
        private PhotonPacketHandler photonPacketHandler;
        private CommandContext ctx;

        public LootLogger(CommandContext ctx)
        {
            this._lootService = new LootService();
            this._eventHandler = new PacketHandler(this._lootService);
            this.photonPacketHandler = new PhotonPacketHandler(this._eventHandler);
            this.ctx = ctx;

            new Thread(delegate ()
            {
                this.CreateListener();
            }).Start();
            ctx.Channel.SendMessageAsync("Starting Network monitoring");
            //Console.WriteLine(Strings.WelcomeMessage);
        }

        public void SaveLootsToFile()
        {
            _lootService.SaveLootsToFile();
        }

        public void GetLoots()
        {
            _lootService.GetLoots(this.ctx);
        }

        private void CreateListener()
        {
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;
            if (allDevices.Count == 0)
            {
                Debug.WriteLine("No Network Interface Found! Please make sure WinPcap is properly installed.");
                ctx.Channel.SendMessageAsync("No Network Interface Found! Please make sure WinPcap is properly installed.");
                return;
            }
            for (int i = 0; i != allDevices.Count; i++)
            {
                LivePacketDevice device = allDevices[i];
                if (device.Description != null)
                {
                    Debug.WriteLine(" (" + device.Description + ")");
                }
                else
                {
                    Debug.WriteLine(" (Unknown)");
                }
            }
            using (List<LivePacketDevice>.Enumerator enumerator = allDevices.ToList<LivePacketDevice>().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    PacketDevice selectedDevice = enumerator.Current;
                    new Thread(delegate ()
                    {
                        using (PacketCommunicator communicator = selectedDevice.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                        {
                            if (communicator.DataLink.Kind != DataLinkKind.Ethernet)
                            {
                                Debug.WriteLine("This program works only on Ethernet networks.");
                                ctx.Channel.SendMessageAsync("This program works only on Ethernet networks.");
                            }
                            else
                            {
                                using (BerkeleyPacketFilter filter = communicator.CreateFilter("ip and udp"))
                                {
                                    communicator.SetFilter(filter);
                                }
                                Console.WriteLine("Capturing on " + selectedDevice.Description + "...");
                                communicator.ReceivePackets(0, new HandlePacket(photonPacketHandler.PacketHandler));
                            }
                        }
                    }).Start();
                }
            }
        }
    }
}
