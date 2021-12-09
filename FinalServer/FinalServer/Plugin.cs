using DarkRift;
using DarkRift.Server;
using FinalCommon;
using FinalCommon.Data;
using System;

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

        // Entry point - plugin is initialized by DarkRift
        public FinalServerPlugin(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            Game = new GameController();
            pluginLoadData.ClientManager.ClientConnected += _clientConnectedHandler;
            pluginLoadData.ClientManager.ClientDisconnected += _clientDisconnectedHandler;
        }

        // If room in game, adds new clients to it and adds message received handler
        private void _clientConnectedHandler(object sender, ClientConnectedEventArgs args)
        {
            args.Client.MessageReceived += _messageReceivedHandler;

            Game.ClientConnectedHandler(args.Client);
        }

        private void _clientDisconnectedHandler(object sender, ClientDisconnectedEventArgs args)
        {
            Game.ClientDisconnectedHandler(args.Client);
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
    internal static class ClientExtensions
    {
        public static void SendMessage<T>(this IClient client, T data, SendMode mode = SendMode.Reliable) where T : IDarkRiftSerializable, IServerToClient
        {
            client.SendMessage(data.ServerToClientTag, data, mode);
        }

        public static void SendMessage<T>(this IClient client, ServerToClient tag, T data, SendMode mode = SendMode.Reliable) where T : IDarkRiftSerializable
        {
            using (var writer = DarkRiftWriter.Create())
            {
                writer.Write(data);
                using (var message = Message.Create((ushort)tag, writer))
                {
                    client.SendMessage(message, mode);
                }
            }
        }
    }
}
