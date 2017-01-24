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
        private void MenuUpdate(GameTime gameTime)
        {
            bool mouseInView = true;
            // mouse stuff
            if ((Mouse.GetState().X >= 0 && Mouse.GetState().X < GraphicsDevice.Viewport.Width) && (Mouse.GetState().Y >= 0 && Mouse.GetState().Y < GraphicsDevice.Viewport.Height))
            {
                mouseX = Mouse.GetState().X;
                mouseY = Mouse.GetState().Y;
            }
            else
            {
                mouseInView = false;
            }
            cursorPosition = new Vector2(mouseX - 37, mouseY - 37);

            if (mouseInView && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                currentState = GameState.Rail;
            }
        }

        private void MenuDraw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(0,128,128,255));

            DrawModel(playerModel, playerPos * Matrix.CreateRotationY((float)Math.PI) * Matrix.CreateTranslation(new Vector3(0, 2, 0)), camera, projection);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(frameTexture, new Vector2(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 - 100), Color.White);
            spriteBatch.Draw(buttonPlayTexture, new Vector2(GraphicsDevice.Viewport.Width / 5 + 10, GraphicsDevice.Viewport.Height / 2 - 100 + 75), Color.White);
            spriteBatch.Draw(buttonResetTexture, new Vector2(GraphicsDevice.Viewport.Width / 5 + 10, GraphicsDevice.Viewport.Height / 2 - 100 + 100), Color.White);
            spriteBatch.Draw(buttonOptionsTexture, new Vector2(GraphicsDevice.Viewport.Width / 5 + 10, GraphicsDevice.Viewport.Height / 2 - 100 + 125), Color.White);
            spriteBatch.Draw(buttonExitTexture, new Vector2(GraphicsDevice.Viewport.Width / 5 + 10, GraphicsDevice.Viewport.Height / 2 - 100 + 150), Color.White);
            spriteBatch.Draw(cursorTexture, cursorPosition, Color.White);
            spriteBatch.End();
        }
    }
}
