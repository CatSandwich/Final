using System;

using DarkRift;
using DarkRift.Server;

using FinalCommon;
using FinalCommon.Data;
using FinalServer.Data;

namespace FinalServer
{
    public class FinalServerPlugin : Plugin
    {
        public override bool ThreadSafe => false;
        public override Version Version => new Version(1, 0, 0);

        public FinalServerPlugin(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            pluginLoadData.ClientManager.ClientConnected += _clientConnectedHandler;
        }

        private void _clientConnectedHandler(object sender, ClientConnectedEventArgs args)
        {
            args.Client.MessageReceived += _messageReceivedHandler;
            Console.WriteLine("Client connected - yay!!!");
        }

        private void _messageReceivedHandler(object sender, MessageReceivedEventArgs args)
        {
            Console.WriteLine("Message received - yay!!!");
            args.Client.SendMessage(0, new TestData() { FirstName = "Ree", LastName = "TS", Age = 42 });

            switch ((ClientToServer)args.Tag)
            {
                case ClientToServer.PaddlePosition:
                    {
                        _handlePaddlePosition(args.GetMessage().Deserialize<PaddlePositionData>());
                        break;
                    }
            }
        }

        private void _handlePaddlePosition(PaddlePositionData data)
        {
            
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
