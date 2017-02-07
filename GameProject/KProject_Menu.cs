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
        int selectedItem = 0;

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
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), resolutionX / resolutionY, 0.1f, 300f);

            if (mouseInView && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                currentState = GameState.Rail;
                startingTime = timer.Now.Ticks;
                beatsElapsed = 0;
                cursorTexture = cursorRail;
            }
        }

        private void MenuDraw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(104, 50, 0, 255));


            DrawModel(playerModel, world * Matrix.CreateScale(new Vector3(0.6f, 1f, 1f)) * Matrix.CreateRotationY((float)Math.PI + 0.1f) * Matrix.CreateTranslation(new Vector3(0.5f, 2, 0)), view, projection);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            spriteBatch.Draw(vignetteTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.Black);
            spriteBatch.Draw(cursorTexture, cursorPosition, Color.White);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            spriteBatch.DrawString(menuFont, "start game", new Vector2(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 - 25), Color.White);
            spriteBatch.DrawString(menuFont, "reset game", new Vector2(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2), Color.White);
            spriteBatch.DrawString(menuFont, "options", new Vector2(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 + 25), Color.White);
            spriteBatch.DrawString(menuFont, "exit", new Vector2(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 + 50), Color.White);
            spriteBatch.DrawString(Arial12, rootNode.Company, new Vector2(5, 120), Color.White);
            spriteBatch.DrawString(Arial12, rootNode.Country, new Vector2(5, 140), Color.White);
            spriteBatch.DrawString(Arial12, rootNode.Address, new Vector2(5, 160), Color.White);
            DrawTree(rootNode);
            spriteBatch.DrawString(scoreFont, "OpenK", new Vector2(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 - 170), Color.White);

            spriteBatch.End();
        }
    }
}
