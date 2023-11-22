using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using static Raylib_cs.Raylib;
namespace GameData
{
    public class Player
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 previousPostition;
        public Vector2 previousVelocity;
        public bool isReady = false;
        public ushort id;

        public EventHandler? playerStartMoveDownEvent;
        public EventHandler? playerStartMoveUpEvent;

        public EventHandler? playerStopMoveEvent;

        public Player() {
            Position = new();
            Velocity = new();
            previousPostition = new();
            previousPostition = new();
        }

        public bool IsMoving()
        {
            return Velocity != new Vector2(0, 0);
        }

        public bool HasMoved()
        {
            return Velocity != previousVelocity;
        }

        public void StartMoveUp()
        {
            if (Position.Y + 20 > 0)
            {
                previousVelocity = Velocity;
                Velocity.X = 0;
                Velocity.Y -= 5;
                playerStartMoveUpEvent?.Invoke(this, new EventArgs());
            }
            else
            {
                StopMove();
                Position.X = 0;
                Position.Y = 20;
            }
            
        }

        public void StartMoveDown()
        {
            
            if (Position.Y + 20 < 600)
            {
                previousVelocity = Velocity;
                Velocity.X = 0;
                Velocity.Y += 5;
                playerStartMoveDownEvent?.Invoke(this, new EventArgs());
            }
            else
            {
                StopMove();
                Position.X = 0;
                Position.Y = 780;
            }
            
        }
        public void StopMove()
        {
            previousVelocity = Velocity;
            Velocity.X = 0;
            Velocity.Y = 0;
            playerStopMoveEvent?.Invoke(this, new EventArgs());
        }

        public void Update()
        {
            
            Position += Velocity;
        }

        public void Draw()
        {
            DrawRectangle((int)Position.X, (int)Position.Y, 10, 20, Color.RED);
        }

        public override bool Equals(object? obj)
        {
            return obj is Player player &&
                   id == player.id;
        }
    }
}
