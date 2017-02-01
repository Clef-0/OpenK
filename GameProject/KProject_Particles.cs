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
                             Speed = new Range<float>(0f, 0f), 
                             Quantity = 1, 
                             Rotation = new Range<float>(-(float)Math.PI, (float)Math.PI), 
                             Scale = 400.0f
                         }, 
                         Modifiers = new IModifier[] 
                         { 
                             new AgeModifier 
                             { 
                                 Interpolators = new IInterpolator[] 
                                 { 
                                     new ColorInterpolator { InitialColor = new HslColor(1f, 1f, 1f), FinalColor = new HslColor(0f, 0f, 1f) },
                                     new ScaleInterpolator { StartValue = new Vector2(400.0f, 100.0f), EndValue = new Vector2(0.0f, 4000.0f) },
                                     new OpacityInterpolator { StartValue = 0.01f, EndValue = 255f }
                                 } 
                             }, 
                             new RectangleContainerModifier {  Width = resolutionX, Height = resolutionY }, 
                             new LinearGravityModifier { Direction = Vector2.UnitY, Strength = 0f } 
                         }
                     } 
                 }
            };
        }
    }
}
