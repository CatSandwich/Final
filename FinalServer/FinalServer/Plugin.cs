using System;
using System.Threading.Tasks;

using DarkRift;
using DarkRift.Server;

using FinalCommon;
using FinalCommon.Data;

namespace FinalServer
{
    // The logic of the server
    public class FinalServerPlugin : Plugin
    {
        // Tells DarkRift that our code is thread-safe, allowing for multi-threading optimization
        public override bool ThreadSafe => true;
        // Tells DarkRift our game's version - unused
        public override Version Version => new Version(1, 0, 0);

        // The game is all controlled through this object
        public GameController Game;

        // Entry point
        public FinalServerPlugin(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            Game = new GameController();
            pluginLoadData.ClientManager.ClientConnected += _clientConnectedHandler;
        }

        // If room in game, adds new clients to it and adds message received handler
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

        // Verifies tag then redirects to the appropriate handler
        private void _messageReceivedHandler(object sender, MessageReceivedEventArgs args)
        {
            if (args.Tag != (ushort)ClientToServer.PaddlePosition)
            {
                Console.WriteLine($"[WARNING] Unknown tag received: {args.Tag}");
                return;
            }

            var data = args.GetMessage().Deserialize<PaddlePositionData>();
            Game.PaddlePositionHandler(args.Client, data);
        }
    }

    // Easier to use SendMessage methods
    static class ClientExtensions
    {
        public static void SendMessage<T>(this IClient client, ServerToClient tag, T data, SendMode mode = SendMode.Reliable) where T : IDarkRiftSerializable 
            => client.SendMessage((ushort)tag, data, mode);
        public static void SendMessage<T>(this IClient client, ushort tag, T data, SendMode mode = SendMode.Reliable) where T : IDarkRiftSerializable
        {
            using (var writer = DarkRiftWriter.Create())
            {
                writer.Write(data);
                using (var message = Message.Create(tag, writer))
                {
                    client.SendMessage(message, mode);
                }
            }
        }
    }
}
