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
    abstract class Enemy
    {
        public Model Model { get; set; }
        public Vector3 Position { get; private set; }
        public Matrix WorldMatrix { get; set; }
        public int Health { get; protected set; }
        protected decimal Velocity;

        public Enemy(Vector3 Position)
        {
            this.Position = Position;
        }

        public void Injure(int HealthDeduction)
        {
            Health -= HealthDeduction;
            if (Health <= 0)
            {
                Health = 0;
            }
        }

        public void FlyTowardsCamera()
        {
            float actualVelocity = 10;
            if (Position.Z > -50)
            {
                actualVelocity = (float)(Velocity / 5);
            }

            Position = new Vector3(Position.X, Position.Y, Position.Z + actualVelocity);
        }
    }

    class Drone : Enemy
    {
        public Drone(Vector3 Position) : base(Position)
        {
            Health = 1;
            Velocity = 2M;
            Model = KProject.droneModel;
        }
    }

    class Sentinel : Enemy
    {
        public Sentinel(Vector3 Position) : base(Position)
        {
            Health = 2;
            Velocity = 4M;
            Model = KProject.sentinelModel;
        }
    }

    class Colonel : Enemy
    {
        public Colonel(Vector3 Position) : base(Position)
        {
            Health = 2;
            Velocity = 5M;
            Model = KProject.colonelModel;
        }
    }
}
