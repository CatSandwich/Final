using System;
using System.Threading.Tasks;

using DarkRift;
using DarkRift.Server;

using FinalCommon;
using FinalCommon.Data;

namespace FinalServer
{
    public class FinalServerPlugin : Plugin
    {
        public override bool ThreadSafe => true;
        public override Version Version => new Version(1, 0, 0);

        public GameController Game;


        public FinalServerPlugin(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            Game = new GameController();
            pluginLoadData.ClientManager.ClientConnected += _clientConnectedHandler;
        }

        private void _clientConnectedHandler(object sender, ClientConnectedEventArgs args)
        {
            if (Game.LeftClient == null)
            {
                args.Client.MessageReceived += _messageReceivedHandler;
                Game.LeftClient = args.Client;
            }
            else if (Game.RightClient == null)
            {
                args.Client.MessageReceived += _messageReceivedHandler;
                Game.RightClient = args.Client;
                Game.GameTask = Task.Run(Game.Run);
            }
            else Console.WriteLine("Client overflow - ignoring new client.");
        }

        private void _messageReceivedHandler(object sender, MessageReceivedEventArgs args)
        {
            switch ((ClientToServer)args.Tag)
            {
                case ClientToServer.PaddlePosition:
                    {
                        Game.PaddlePositionHandler(args.Client, args.GetMessage().Deserialize<PaddlePositionData>().Position);
                        break;
                    }
            }
        }
    }

    static class ClientExtensions
    {
        public static void SendMessage<T>(this IClient client, ushort tag, T data, SendMode mode = SendMode.Reliable) where T : IDarkRiftSerializable
        {
            using (var writer = DarkRiftWriter.Create())
            {
                writer.Write(data);
                client.SendMessage(Message.Create(tag, writer), mode);
            }
        }
    }
}
