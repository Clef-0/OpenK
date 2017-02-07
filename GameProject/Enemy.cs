using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace GameProject
{
    enum EnemyFlightMode { Straight, CurveIn, CurveDown };

    abstract class Enemy
    {
        public Model Model { get; set; }
        public Vector3 Position { get; protected set; }
        public Vector3 Rotation { get; protected set; }
        public Matrix WorldMatrix { get; set; }
        public int Health { get; protected set; }
        protected decimal Velocity;
        public int Points { get; protected set; }
        public EnemyFlightMode FlightMode { get; protected set; }

        public Enemy(Vector3 Position, Vector3 Rotation, EnemyFlightMode FlightMode)
        {
            this.Position = Position;
            this.Rotation = Rotation;
            this.FlightMode = FlightMode;
        }

        public void Injure(int HealthDeduction)
        {
            Health -= HealthDeduction;
            if (Health <= 0)
            {
                Health = 0;
            }
        }

        public void Update()
        {
            float curveval = 0.001f;
            float actualVelocity = (float)(Velocity / 5);
            if (Position.Z > -40)
            {
                actualVelocity = (float)(Velocity / 5) * (Math.Abs(Position.Z) + 40) / 40;
                curveval *= 2;
            }

            if (FlightMode == EnemyFlightMode.CurveIn)
            {
                if (Position.X < 0)
                {
                    Rotation = new Vector3(Rotation.X - curveval, Rotation.Y - (0.5f * curveval), Rotation.Z);
                }
                else
                {
                    Rotation = new Vector3(Rotation.X - curveval, Rotation.Y + (0.5f * curveval), Rotation.Z);
                }
            }
            else if (FlightMode == EnemyFlightMode.CurveDown)
            {
                if (Position.Z > -80)
                {
                    Rotation = new Vector3(-Math.Abs(Math.Abs(Rotation.X) + ((100 + 2 * Math.Abs(Position.Z))/100 * curveval)), Rotation.Y, Rotation.Z);
                    actualVelocity /= Math.Abs(Position.Z) / 40;
                }
            }

            Position = new Vector3(Position.X, Position.Y, Position.Z + actualVelocity);
        }
    }

    class Drone : Enemy
    {
        public Drone(Vector3 Position, Vector3 Rotation, EnemyFlightMode FlightMode) : base(Position, Rotation, FlightMode)
        {
            Health = 1;
            Velocity = 2.5M;
            Model = KProject.droneModel;
            Points = 10;
        }
    }

    class Sentinel : Enemy
    {
        public Sentinel(Vector3 Position, Vector3 Rotation, EnemyFlightMode FlightMode) : base(Position, Rotation, FlightMode)
        {
            Health = 2;
            Velocity = 3.5M;
            Model = KProject.sentinelModel;
            Points = 20;
        }
    }

    class Colonel : Enemy
    {
        public Colonel(Vector3 Position, Vector3 Rotation, EnemyFlightMode FlightMode) : base(Position, Rotation, FlightMode)
        {
            Health = 2;
            Velocity = 4.5M;
            Model = KProject.colonelModel;
            Points = 30;
        }
    }
}
