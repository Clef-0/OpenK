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
        string hoverCompany = "";
        string hoverCountry = "";
        string hoverAddress = "";

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

            Node hoverNode = FindNode(new Point(mouseX, mouseY), rootNode);
            if (hoverNode != null)
            {
                hoverCompany = hoverNode.Company;
                hoverCountry = hoverNode.Country;
                hoverAddress = hoverNode.Address;
            }
            else
            {
                hoverCompany = "";
                hoverCountry = "";
                hoverAddress = "";
            }

            cursorPosition = new Vector2(mouseX - 37, mouseY - 37);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), resolutionX / resolutionY, 0.1f, 300f);

            Rectangle startGame = new Rectangle(new Point(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 - 25), menuFont.MeasureString("start game").ToPoint());
            Rectangle resetGame = new Rectangle(new Point(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2), menuFont.MeasureString("reset game").ToPoint());
            Rectangle marathonMode = new Rectangle(new Point(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 + 25), menuFont.MeasureString("marathon mode").ToPoint());
            Rectangle exit = new Rectangle(new Point(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 + 50), menuFont.MeasureString("exit").ToPoint());
            
            if (mouseInView && Mouse.GetState().LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
            {
                if (hoverNode != null && currentState == GameState.Map)
                {
                    currentState = GameState.Rail;
                    startingTime = timer.Now.Ticks;
                    beatsElapsed = 0;
                    currentNodeColor = hoverNode.Colour;
                    currentNodeMusic = hoverNode.Music;
                    currentNodeScore = 0;
                    currentNodeSpawned = 0;
                    currentNodeShot = 0;
                    enemies.Clear();
                    pEffectExplosion.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    pEffectLock.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    log[0] = "";
                    log[1] = "";
                    log[2] = "";
                    log[3] = "";
                    log[4] = "";
                    cursorTexture = cursorRail;
                }

                if (startGame.Contains(mouseX, mouseY) && currentState == GameState.Menu)
                {
                    currentState = GameState.Map;
                }
                else if (startGame.Contains(mouseX, mouseY) && currentState == GameState.Map)
                {
                    currentState = GameState.Menu;
                }
                else if (resetGame.Contains(mouseX, mouseY) && currentState == GameState.Menu)
                {
                    rootNode.Company = NameGenerate(1);
                    rootNode.Country = NameGenerate(2);
                    rootNode.Address = NameGenerate(3);
                    rootNode.Children.Clear();
                    railNodesMade = 0;
                    CreateChildren(rootNode, 3, 4, 0);
                }
                else if (marathonMode.Contains(mouseX, mouseY))
                {
                    currentState = GameState.Marathon;
                    startingTime = timer.Now.Ticks;
                    beatsElapsed = 0;
                    currentNodeColor = Color.Goldenrod;
                    currentNodeMusic = NodeMusic.Fear;
                    currentNodeScore = 0;
                    currentNodeSpawned = 0;
                    currentNodeShot = 0;
                    enemies.Clear();
                    pEffectExplosion.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    pEffectLock.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    log[0] = "";
                    log[1] = "";
                    log[2] = "";
                    log[3] = "";
                    log[4] = "";
                    cursorTexture = cursorRail;
                }
                else if (exit.Contains(mouseX, mouseY))
                {
                    //SaveGame();
                    Exit();
                }
            }

            oldMouseState = Mouse.GetState();
        }

        private void MenuDraw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(104, 50, 0, 255));

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            if (currentState == GameState.Menu)
            {
                DrawModel(playerModel, world * Matrix.CreateScale(new Vector3(0.6f, 1f, 1f)) * Matrix.CreateRotationY((float)Math.PI + 0.1f) * Matrix.CreateTranslation(new Vector3(0.5f, 2, 0)), view, projection);
                spriteBatch.DrawString(menuFont, "start game", new Vector2(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 - 25), Color.White);
                spriteBatch.DrawString(menuFont, "reset game", new Vector2(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2), Color.White);
                spriteBatch.DrawString(menuFont, "marathon mode", new Vector2(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 + 25), Color.White);
                spriteBatch.DrawString(menuFont, "exit", new Vector2(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 + 50), Color.White);
            }
            else
            {
                spriteBatch.DrawString(menuFont, "back", new Vector2(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 - 25), Color.White);
                if (hoverCompany != "")
                {
                    spriteBatch.Draw(linePixel, new Rectangle(resolutionX - 400, resolutionY / 2 + 20, (int)menuFont.MeasureString(hoverCompany).X + 20, 20), null, Color.FromNonPremultiplied(100, 100, 100, 100), 0, new Vector2(0, 0), SpriteEffects.None, 0);
                    spriteBatch.DrawString(menuFont, hoverCompany, new Vector2(resolutionX - 400, resolutionY / 2 + 20), Color.White);
                    spriteBatch.Draw(linePixel, new Rectangle(resolutionX - 400, resolutionY / 2 + 40, (int)menuFont.MeasureString(hoverCountry).X + 20, 20), null, Color.FromNonPremultiplied(100, 100, 100, 100), 0, new Vector2(0, 0), SpriteEffects.None, 0);
                    spriteBatch.DrawString(menuFont, hoverCountry, new Vector2(resolutionX - 400, resolutionY / 2 + 40), Color.White);
                    spriteBatch.Draw(linePixel, new Rectangle(resolutionX - 400, resolutionY / 2 + 60, (int)menuFont.MeasureString(hoverAddress).X + 20, 20), null, Color.FromNonPremultiplied(100, 100, 100, 100), 0, new Vector2(0, 0), SpriteEffects.None, 0);
                    spriteBatch.DrawString(menuFont, hoverAddress, new Vector2(resolutionX - 400, resolutionY / 2 + 60), Color.White);
                }
               
                DrawTree(rootNode);
            }
            spriteBatch.DrawString(scoreFont, "OpenK", new Vector2(GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 2 - 170), Color.White);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            spriteBatch.Draw(vignetteTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.Black);
            spriteBatch.Draw(cursorTexture, cursorPosition, Color.White);

            spriteBatch.End();

            // return spritebatch to Additive mode for 3D
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.End();
        }
    }
}
