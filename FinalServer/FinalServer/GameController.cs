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
        public const float Speed = 3f;
        // The delay in milliseconds between each game tick
        public const int TickSpeed = 20;
        // The bounds of the game
        public static readonly Vector2 GameWorldSize = new Vector2(10, 5);

        // Stores the sign of the ball's velocity
        public Vector2 BallDirection = new Vector2(1, 1);
        // Calculates the ball's velocity
        public Vector2 CalculatedSpeed => BallDirection * Speed * TickSpeed / 1000f;

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

        // Updates player's paddle's position
        public void PaddlePositionHandler(IClient client, PaddlePositionData pos)
        {
            if (!_init) return;

            lock (this)
            {
                (client.ID == LeftClient.ID ? LeftPaddle : RightPaddle).Position.Y = pos.Position.Y;
            }
        }

        // Core game logic
        public async void Run()
        {
            // Initialize game world
            _resize(ObjectIds.Ball, Ball.Size);
            _resize(ObjectIds.LeftPaddle, LeftPaddle.Size);
            _resize(ObjectIds.RightPaddle, RightPaddle.Size);
            _resize(ObjectIds.Camera, GameWorldSize);
            _move(ObjectIds.Camera, new Vector3(GameWorldSize.X / 2, GameWorldSize.Y / 2, -10));
            _move(ObjectIds.Ball, GameWorldSize / 2);
            _move(ObjectIds.LeftPaddle, LeftPaddle.Position);
            _move(ObjectIds.RightPaddle, RightPaddle.Position);

            LeftClient.SendMessage(ServerToClient.SetOwnership, new SetOwnershipData { Id = ObjectIds.LeftPaddle });
            RightClient.SendMessage(ServerToClient.SetOwnership, new SetOwnershipData { Id = ObjectIds.RightPaddle });

            // Wait 3 seconds for players to get ready
            await Task.Delay(3000);
            _init = true;

            // Core game loop
            while (true)
            {
                Ball.Position += CalculatedSpeed;

                // Paddle collision
                if (Ball.CheckCollision(LeftPaddle)) BallDirection.X = 1;
                if (Ball.CheckCollision(RightPaddle)) BallDirection.X = -1;

                // Outer horizontal bounds
                if(Ball.Position.X <= 0)
                {
                    Console.WriteLine("Left paddle lost");
                    Ball.Position = GameWorldSize / 2;
                    _move(ObjectIds.Ball, Ball.Position);
                    await Task.Delay(1000);
                }
                if (Ball.Position.X >= GameWorldSize.X)
                {
                    Console.WriteLine("Right paddle lost");
                    Ball.Position = GameWorldSize / 2;
                    _move(ObjectIds.Ball, Ball.Position);
                    await Task.Delay(1000);
                }

                // Outer vertical bounds
                if (Ball.Position.Y + Ball.Size.Y / 2 >= GameWorldSize.Y) BallDirection.Y = -1;
                if (Ball.Position.Y - Ball.Size.Y / 2 <= 0) BallDirection.Y = 1;

                // Broadcast new information
                _move(ObjectIds.Ball, Ball.Position);
                LeftClient.SendMessage((ushort)ServerToClient.MoveObject, new MoveObjectData { Id = ObjectIds.RightPaddle, Position = RightPaddle.Position });
                RightClient.SendMessage((ushort)ServerToClient.MoveObject, new MoveObjectData { Id = ObjectIds.LeftPaddle, Position = LeftPaddle.Position });

                // Sleep to enforce tick rate
                await Task.Delay(TickSpeed);
            }
        }

        // Broadcasts the new position, pos, of an object, obj
        private void _move(ObjectIds obj, Vector3 pos) => _send(ServerToClient.MoveObject, new MoveObjectData(obj, pos));
        // Broadcasts the new size, size, of an object, obj
        private void _resize(ObjectIds obj, Vector2 size) => _send(ServerToClient.ResizeObject, new ResizeObjectData(obj, size));

        // Broadcasts a generic message
        private void _send<T>(ServerToClient tag, T data, SendMode mode = SendMode.Reliable) where T : IDarkRiftSerializable
        {
            LeftClient.SendMessage(tag, data, mode);
            RightClient.SendMessage(tag, data, mode);
        }
    }
}
