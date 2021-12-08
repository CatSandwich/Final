using DarkRift.Server;

using FinalCommon;
using FinalCommon.Data;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkRift;

namespace FinalServer
{
    public class GameController
    {
        public static readonly Vector2 GameWorldSize = new Vector2 { X = 10, Y = 5 };
        public const float PaddleOffset = 1f;

        public const float Speed = 3f;
        public const int TickSpeed = 20;

        public Vector2 BallDirection = new Vector2{X = 1, Y = 1};
        public Vector2 CalculatedSpeed => BallDirection * Speed * TickSpeed / 1000f;

        public Task GameTask;

        public Box Ball = new Box { Position = new Vector3 { X = GameWorldSize.X / 2, Y = GameWorldSize.Y / 2 }, Size = new Vector2{X = 0.25f, Y = 0.25f} };

        public IClient LeftClient;
        public IClient RightClient;

        private bool _init = false;

        public Paddle LeftPaddle = new Paddle 
        { 
            Position = new Vector3 { X = PaddleOffset, Y = GameWorldSize.Y / 2, Z = 0 }, 
            Size = new Vector2 { X = 0.5f, Y = 1f }, 
            Side = PaddleSide.Left 
        };
        public Paddle RightPaddle = new Paddle 
        { 
            Position = new Vector3 { X = GameWorldSize.X - PaddleOffset, Y = GameWorldSize.Y / 2, Z = 0 }, 
            Size = new Vector2 { X = 0.5f, Y = 1f }, 
            Side = PaddleSide.Right 
        };

        public void PaddlePositionHandler(IClient client, Vector3 newPos)
        {
            if (!_init) return;

            if(client.ID == LeftClient.ID)
            {
                lock (this)
                {
                    LeftPaddle.Position.Y = newPos.Y;
                    return;
                }
            }

            if(client.ID == RightClient.ID)
            {
                lock (this)
                {
                    RightPaddle.Position.Y = newPos.Y;
                    return;
                }
            }

            Console.WriteLine("[ERROR] Trying to accept input from client not in game.");
        }

        public async void Run()
        {
            // Initialize game world
            _resize(ObjectIds.Ball, Ball.Size);
            _resize(ObjectIds.LeftPaddle, LeftPaddle.Size);
            _resize(ObjectIds.RightPaddle, RightPaddle.Size);
            _resize(ObjectIds.Camera, GameWorldSize);
            _move(ObjectIds.Camera, new Vector3{X = GameWorldSize.X / 2, Y = GameWorldSize.Y / 2, Z = -10});
            _move(ObjectIds.Ball, GameWorldSize / 2);
            _move(ObjectIds.LeftPaddle, LeftPaddle.Position);
            _move(ObjectIds.RightPaddle, RightPaddle.Position);

            LeftClient.SendMessage(ServerToClient.SetOwnership, new SetOwnershipData { Id = ObjectIds.LeftPaddle });
            RightClient.SendMessage(ServerToClient.SetOwnership, new SetOwnershipData { Id = ObjectIds.RightPaddle });

            await Task.Delay(3000);

            _init = true;

            while (true)
            {
                Ball.Position += CalculatedSpeed;

                if(Ball.Position.X - Ball.Size.X / 2 <= LeftPaddle.Position.X + LeftPaddle.Size.X / 2 && Ball.CheckYAxisCollision(LeftPaddle))
                {
                    BallDirection.X = 1;
                }

                if(Ball.Position.X + Ball.Size.X / 2 >= RightPaddle.Position.X - RightPaddle.Size.X / 2 && Ball.CheckYAxisCollision(RightPaddle))
                {
                    BallDirection.X = -1;
                }

                if(Ball.Position.X <= 0)
                {
                    Console.WriteLine("Left paddle lost");
                    Ball.Position = GameWorldSize / 2;
                }

                if (Ball.Position.X >= GameWorldSize.X)
                {
                    Console.WriteLine("Right paddle lost");
                    Ball.Position = GameWorldSize / 2;
                }

                if (Ball.Position.Y + Ball.Size.Y / 2 >= GameWorldSize.Y) BallDirection.Y = -1;
                if (Ball.Position.Y - Ball.Size.Y / 2 <= 0) BallDirection.Y = 1;

                _move(ObjectIds.Ball, Ball.Position);
                LeftClient.SendMessage((ushort)ServerToClient.MoveObject, new MoveObjectData { Id = ObjectIds.RightPaddle, Position = RightPaddle.Position });
                RightClient.SendMessage((ushort)ServerToClient.MoveObject, new MoveObjectData { Id = ObjectIds.LeftPaddle, Position = LeftPaddle.Position });

                await Task.Delay(TickSpeed);
            }
        }

        private void _move(ObjectIds obj, Vector3 pos) => _send(ServerToClient.MoveObject, new MoveObjectData(obj, pos));
        private void _resize(ObjectIds obj, Vector2 size) => _send(ServerToClient.ResizeObject, new ResizeObjectData(obj, size));

        private void _send<T>(ServerToClient tag, T data, SendMode mode = SendMode.Reliable) where T : IDarkRiftSerializable
        {
            LeftClient.SendMessage(tag, data, mode);
            RightClient.SendMessage(tag, data, mode);
        }
    }
}
