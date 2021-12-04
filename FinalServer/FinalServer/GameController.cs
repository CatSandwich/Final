using DarkRift.Server;

using FinalCommon;
using FinalCommon.Data;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinalServer
{
    public class GameController
    {
        public static readonly Vector2 GameWorldSize = new Vector2() { X = 10, Y = 5 };
        public const float PaddleOffset = 1f;

        public const float Speed = 3f;
        public const int TickSpeed = 50;

        public int BallDirection = 1;

        public float CalculatedSpeed => Speed * BallDirection * TickSpeed / 1000f;

        public Task GameTask;

        public Box Ball = new Box { Position = new Vector3 { X = GameWorldSize.X / 2, Y = GameWorldSize.Y / 2 } };

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
                lock (LeftPaddle)
                {
                    LeftPaddle.Position = newPos;
                    return;
                }
            }

            if(client.ID == RightClient.ID)
            {
                lock (RightPaddle)
                {
                    RightPaddle.Position = newPos;
                    return;
                }
            }

            Console.WriteLine("Trying to accept input from client not in game.");
        }

        public async void Run()
        {
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
                }

                if(Ball.Position.X >= GameWorldSize.X)
                {
                    Console.WriteLine("Right paddle lost");
                }

                LeftClient.SendMessage((ushort)ServerToClient.MoveObject, new MoveObjectData { Id = ObjectIds.RightPaddle, Position = RightPaddle.Position });
                LeftClient.SendMessage((ushort)ServerToClient.MoveObject, new MoveObjectData { Id = ObjectIds.Ball, Position = Ball.Position });

                RightClient.SendMessage((ushort)ServerToClient.MoveObject, new MoveObjectData { Id = ObjectIds.LeftPaddle, Position = LeftPaddle.Position });
                RightClient.SendMessage((ushort)ServerToClient.MoveObject, new MoveObjectData { Id = ObjectIds.Ball, Position = Ball.Position });
                await Task.Delay(TickSpeed);
            }
        }
    }
}
