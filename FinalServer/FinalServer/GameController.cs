using DarkRift;
using DarkRift.Server;
using FinalCommon;
using FinalCommon.Data;
using System;
using System.Threading.Tasks;

namespace FinalServer
{
    // Controls a game of pong between two external clients
    public class GameController
    {
        // The distance from outer horizontal bounds to the paddle
        public const float PaddleOffset = 1f;
        // The speed in units per second that the ball moves
        public const float Speed = 6f;
        // The delay in milliseconds between each game tick
        public const int TickSpeed = 10;
        // The bounds of the game
        public static readonly Vector2 GameWorldSize = new Vector2(15, 5);

        // Stores the sign of the ball's velocity
        public (int X, int Y) BallDirection = (1, 1);
        // Calculates the ball's velocity
        public Vector2 CalculatedSpeed => new Vector2(BallDirection.X, BallDirection.Y) * Speed * TickSpeed / 1000f;

        // Stores the task object encapsulating the game
        public Task GameTask;

        // Game objects
        public Box Ball = new Box 
        { 
            Position = new Vector3(GameWorldSize.X / 2, GameWorldSize.Y / 2), 
            Size = new Vector2(0.25f, 0.25f)
        };
        public Box LeftPaddle = new Box
        {
            Position = new Vector3(PaddleOffset, GameWorldSize.Y / 2),
            Size = new Vector2(0.5f, 1f)
        };
        public Box RightPaddle = new Box
        {
            Position = new Vector3(GameWorldSize.X - PaddleOffset, GameWorldSize.Y / 2),
            Size = new Vector2(0.5f, 1f)
        };

        // Clients
        public IClient LeftClient;
        public IClient RightClient;

        // Prevents input until the game world is initialized
        private bool _init = false;

        public GameController()
        {
            GameTask = Task.Run(Run);
        }

        // Updates player's paddle's position
        public void PaddlePositionHandler(IClient client, PaddlePositionData data)
        {
            if (!_init) return;

            lock (this)
            {
                if (LeftClient != null && client.ID == LeftClient.ID) LeftPaddle.Position.Y = data.Position.Y;
                if (RightClient != null && client.ID == RightClient.ID) RightPaddle.Position.Y = data.Position.Y;
            }
        }

        // Registers them in game if room then initializes their game world
        public void ClientConnectedHandler(IClient client)
        {
            if (LeftClient == null)
            {
                LeftClient = client;
                client.SendMessage(new SetOwnershipData(ObjectIds.LeftPaddle));
            }
            else if (RightClient == null)
            {
                RightClient = client;
                client.SendMessage(new SetOwnershipData(ObjectIds.RightPaddle));
            }
            else
            {
                Console.WriteLine("[INFO] Client overflow. Disconnecting new client.");
                client.Disconnect();
                return;
            }

            // Initialize game world
            client.SendMessage(new ResizeObjectData(ObjectIds.Ball, Ball.Size));
            client.SendMessage(new ResizeObjectData(ObjectIds.LeftPaddle, LeftPaddle.Size));
            client.SendMessage(new ResizeObjectData(ObjectIds.RightPaddle, RightPaddle.Size));
            client.SendMessage(new ResizeObjectData(ObjectIds.Camera, GameWorldSize));
            client.SendMessage(new MoveObjectData(ObjectIds.Camera, new Vector3(GameWorldSize.X / 2, GameWorldSize.Y / 2, -10)));
            client.SendMessage(new MoveObjectData(ObjectIds.Ball, GameWorldSize / 2));
            client.SendMessage(new MoveObjectData(ObjectIds.LeftPaddle, LeftPaddle.Position));
            client.SendMessage(new MoveObjectData(ObjectIds.RightPaddle, RightPaddle.Position));
        }

        // Sets appropriate client reference to null
        public void ClientDisconnectedHandler(IClient client)
        {
            if (client?.ID == LeftClient?.ID) LeftClient = null;
            else if (client?.ID == RightClient?.ID) RightClient = null;
        }

        // Core game logic
        public async void Run()
        {
            LeftClient?.SendMessage(new SetOwnershipData(ObjectIds.LeftPaddle));
            RightClient?.SendMessage(new SetOwnershipData(ObjectIds.RightPaddle));

            // Wait 3 seconds for players to get ready
            await Task.Delay(3000);
            _init = true;

            // Core game loop
            while (true)
            {
                // Pause game until two players active
                if (LeftClient == null || RightClient == null)
                {
                    await Task.Delay(TickSpeed);
                    continue;
                }

                Ball.Position += CalculatedSpeed;

                // Paddle collision
                if (BallDirection.X == -1 && Ball.CheckCollision(LeftPaddle)) 
                {
                    BallDirection.X = 1;
                    Console.WriteLine("[COLLISION] Ball - Left Paddle");
                }
                if (BallDirection.X == 1 && Ball.CheckCollision(RightPaddle))
                {
                    BallDirection.X = -1;
                    Console.WriteLine("[COLLISION] Ball - Right Paddle");
                }

                // Outer horizontal bounds
                if(Ball.Position.X <= 0)
                {
                    Console.WriteLine("[SCORE] Right paddle");
                    Ball.Position = GameWorldSize / 2;
                    _send(new MoveObjectData(ObjectIds.Ball, Ball.Position));
                    await Task.Delay(1000);
                }
                if (Ball.Position.X >= GameWorldSize.X)
                {
                    Console.WriteLine("[SCORE] Left paddle");
                    Ball.Position = GameWorldSize / 2;
                    _send(new MoveObjectData(ObjectIds.Ball, Ball.Position));
                    await Task.Delay(1000);
                }

                // Outer vertical bounds
                if (Ball.Position.Y + Ball.Size.Y / 2 >= GameWorldSize.Y)
                {
                    BallDirection.Y = -1;
                    Console.WriteLine("[COLLISION] Ball - Upper Bounds");
                }
                if (Ball.Position.Y - Ball.Size.Y / 2 <= 0)
                {
                    BallDirection.Y = 1;
                    Console.WriteLine("[COLLISION] Ball - Lower Bounds");
                }

                // Broadcast new information
                _send(new MoveObjectData(ObjectIds.Ball, Ball.Position), SendMode.Unreliable);
                LeftClient?.SendMessage(new MoveObjectData(ObjectIds.RightPaddle, RightPaddle.Position), SendMode.Unreliable);
                RightClient?.SendMessage(new MoveObjectData(ObjectIds.LeftPaddle, LeftPaddle.Position), SendMode.Unreliable);

                // Sleep to enforce tick rate
                await Task.Delay(TickSpeed);
            }
        }

        private void _send<T>(T data, SendMode mode = SendMode.Reliable) where T : IDarkRiftSerializable, IServerToClient
        {
            _send(data.ServerToClientTag, data, mode);
        }

        // Broadcasts a generic message
        private void _send<T>(ServerToClient tag, T data, SendMode mode = SendMode.Reliable) where T : IDarkRiftSerializable
        {
            LeftClient?.SendMessage(tag, data, mode);
            RightClient?.SendMessage(tag, data, mode);
        }
    }
}