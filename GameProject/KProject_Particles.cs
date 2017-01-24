using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject
{
    public partial class KProject : Game
    {
        private void ParticleInit(TextureRegion2D textureRegion)
        {
            pEffect = new ParticleEffect
            {
                Emitters = new[] 
                 { 
                     new ParticleEmitter(textureRegion, 5000, TimeSpan.FromSeconds(0.5), Profile.Ring(1f, Profile.CircleRadiation.Out))
                     { 
                         Parameters = new ParticleReleaseParameters 
                         { 
                             Speed = new Range<float>(1f, 500f), 
                             Quantity = 100, 
                             Rotation = new Range<float>(-1f, 1f), 
                             Scale = new Range<float>(30.0f, 40.0f),
                         }, 
                         Modifiers = new IModifier[] 
                         { 
                             new AgeModifier 
                             { 
                                 Interpolators = new IInterpolator[] 
                                 { 
                                     new ColorInterpolator { InitialColor = new HslColor(1f, 1f, 1f), FinalColor = new HslColor(0.9f, 0.9f, 1f) } 
                                 } 
                             }, 
                             new RotationModifier { RotationRate = -2.1f }, 
                             new RectangleContainerModifier {  Width = resolutionX, Height = resolutionY }, 
                             new LinearGravityModifier { Direction = Vector2.UnitY, Strength = 300f } 
                         }
                     } 
                 }
            };
        }
    }
}
