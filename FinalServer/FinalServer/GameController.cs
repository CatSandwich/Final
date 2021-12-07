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
        public static readonly Vector2 GameWorldSize = new Vector2() { X = 10, Y = 5 };
        public const float PaddleOffset = 1f;

        public const float Speed = 3f;
        public const int TickSpeed = 20;

        public int BallDirection = 1;

        public float CalculatedSpeed => Speed * BallDirection * TickSpeed / 1000f;

        public Task GameTask;

        public Box Ball = new Box { Position = new Vector3 { X = GameWorldSize.X / 2, Y = GameWorldSize.Y / 2 }, Size = new Vector2{X = 0.25f, Y = 0.25f} };

        public IClient LeftClient;
        public IClient RightClient;

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
            if(client.ID == LeftClient.ID)
            {
                lock (this)
                {
                    LeftPaddle.Position = newPos;
                    return;
                }
            }

            if(client.ID == RightClient.ID)
            {
                lock (this)
                {
                    RightPaddle.Position = newPos;
                    return;
                }
            }

            Console.WriteLine("[ERROR] Trying to accept input from client not in game.");
        }

        public async void Run()
        {
            // Initialize game world
            _resize(ObjectIds.Ball       , Ball.Size              );
            _resize(ObjectIds.LeftPaddle , LeftPaddle.Size        );
            _resize(ObjectIds.RightPaddle, RightPaddle.Size       );
            _resize(ObjectIds.Camera     , GameWorldSize          );
            _move  (ObjectIds.Camera     , GameWorldSize / 2);
            _move  (ObjectIds.Ball       , GameWorldSize / 2);

            await Task.Delay(3000);

            while (true)
            {
                Ball.Position.X += CalculatedSpeed;

                if(Ball.Position.X <= PaddleOffset && Ball.CheckYAxisCollision(LeftPaddle))
                {
                    BallDirection = 1;
                }

                if(Ball.Position.X >= GameWorldSize.X - PaddleOffset && Ball.CheckYAxisCollision(RightPaddle))
                {
                    BallDirection = -1;
                }

                if(Ball.Position.X <= 0)
                {
                    Console.WriteLine("Left paddle lost");
                    Ball.Position = GameWorldSize / 2;
                }

                if(Ball.Position.X >= GameWorldSize.X)
                {
                    Console.WriteLine("Right paddle lost");
                    Ball.Position = GameWorldSize / 2;
                }
                
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
